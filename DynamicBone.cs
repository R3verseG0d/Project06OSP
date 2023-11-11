using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[AddComponentMenu("Dynamic Bone/Dynamic Bone")]
public class DynamicBone : MonoBehaviour
{
	public enum UpdateMode
	{
		Normal = 0,
		AnimatePhysics = 1,
		UnscaledTime = 2,
		Default = 3
	}

	public enum FreezeAxis
	{
		None = 0,
		X = 1,
		Y = 2,
		Z = 3
	}

	internal class Particle
	{
		public Transform m_Transform;

		public int m_ParentIndex;

		public int m_ChildCount;

		public float m_Damping;

		public float m_Elasticity;

		public float m_Stiffness;

		public float m_Inert;

		public float m_Friction;

		public float m_Radius;

		public float m_BoneLength;

		public bool m_isCollide;

		public bool m_TransformNotNull;

		public Vector3 m_Position;

		public Vector3 m_PrevPosition;

		public Vector3 m_EndOffset;

		public Vector3 m_InitLocalPosition;

		public Quaternion m_InitLocalRotation;

		public Vector3 m_TransformPosition;

		public Vector3 m_TransformLocalPosition;

		public Matrix4x4 m_TransformLocalToWorldMatrix;
	}

	internal class ParticleTree
	{
		public Transform m_Root;

		public Vector3 m_LocalGravity;

		public Matrix4x4 m_RootWorldToLocalMatrix;

		public float m_BoneTotalLength;

		public List<Particle> m_Particles = new List<Particle>();

		public Vector3 m_RestGravity;
	}

	[Tooltip("The roots of the transform hierarchy to apply physics.")]
	public Transform m_Root;

	public List<Transform> m_Roots;

	[Tooltip("Internal physics simulation rate.")]
	public float m_UpdateRate = 60f;

	public UpdateMode m_UpdateMode = UpdateMode.Default;

	[Tooltip("How much the bones slowed down.")]
	[Range(0f, 1f)]
	public float m_Damping = 0.1f;

	public AnimationCurve m_DampingDistrib;

	[Tooltip("How much the force applied to return each bone to original orientation.")]
	[Range(0f, 1f)]
	public float m_Elasticity = 0.1f;

	public AnimationCurve m_ElasticityDistrib;

	[Tooltip("How much bone's original orientation are preserved.")]
	[Range(0f, 1f)]
	public float m_Stiffness = 0.1f;

	public AnimationCurve m_StiffnessDistrib;

	[Tooltip("How much character's position change is ignored in physics simulation.")]
	[Range(0f, 1f)]
	public float m_Inert;

	public AnimationCurve m_InertDistrib;

	[Tooltip("How much the bones slowed down when collide.")]
	public float m_Friction;

	public AnimationCurve m_FrictionDistrib;

	[Tooltip("Each bone can be a sphere to collide with colliders. Radius describe sphere's size.")]
	public float m_Radius;

	public AnimationCurve m_RadiusDistrib;

	[Tooltip("If End Length is not zero, an extra bone is generated at the end of transform hierarchy.")]
	public float m_EndLength;

	[Tooltip("If End Offset is not zero, an extra bone is generated at the end of transform hierarchy.")]
	public Vector3 m_EndOffset = Vector3.zero;

	[Tooltip("The force apply to bones. Partial force apply to character's initial pose is cancelled out.")]
	public Vector3 m_Gravity = Vector3.zero;

	[Tooltip("The force apply to bones.")]
	public Vector3 m_Force = Vector3.zero;

	[Tooltip("Control how physics blends with existing animation.")]
	[Range(0f, 1f)]
	public float m_BlendWeight = 1f;

	[Tooltip("Collider objects interact with the bones.")]
	public List<DynamicBoneColliderBase> m_Colliders;

	[Tooltip("Bones exclude from physics simulation.")]
	public List<Transform> m_Exclusions;

	[Tooltip("Constrain bones to move on specified plane.")]
	public FreezeAxis m_FreezeAxis;

	[Tooltip("Disable physics simulation automatically if character is far from camera or player.")]
	public bool m_DistantDisable;

	public Transform m_ReferenceObject;

	public float m_DistanceToObject = 20f;

	[HideInInspector]
	public bool m_Multithread = true;

	private Vector3 m_ObjectMove;

	private Vector3 m_ObjectPrevPosition;

	private float m_ObjectScale;

	private float m_Time;

	private float m_Weight = 1f;

	private bool m_DistantDisabled;

	private int m_PreUpdateCount;

	internal List<ParticleTree> m_ParticleTrees = new List<ParticleTree>();

	private float m_DeltaTime;

	private List<DynamicBoneColliderBase> m_EffectiveColliders;

	private bool m_WorkAdded;

	private static List<DynamicBone> s_PendingWorks = new List<DynamicBone>();

	private static List<DynamicBone> s_EffectiveWorks = new List<DynamicBone>();

	private static AutoResetEvent s_AllWorksDoneEvent;

	private static int s_RemainWorkCount;

	private static Semaphore s_WorkQueueSemaphore;

	private static int s_WorkQueueIndex;

	private static int s_UpdateCount;

	private static int s_PrepareFrame;

	private void Start()
	{
		SetupParticles();
	}

	private void FixedUpdate()
	{
		if (m_UpdateMode == UpdateMode.AnimatePhysics)
		{
			PreUpdate();
		}
	}

	private void Update()
	{
		if (m_UpdateMode != UpdateMode.AnimatePhysics)
		{
			PreUpdate();
		}
		if (m_PreUpdateCount > 0 && m_Multithread)
		{
			AddPendingWork(this);
			m_WorkAdded = true;
		}
		s_UpdateCount++;
	}

	private void LateUpdate()
	{
		if (m_PreUpdateCount == 0)
		{
			return;
		}
		if (s_UpdateCount > 0)
		{
			s_UpdateCount = 0;
			s_PrepareFrame++;
		}
		SetWeight(m_BlendWeight);
		if (m_WorkAdded)
		{
			m_WorkAdded = false;
			ExecuteWorks();
		}
		else
		{
			CheckDistance();
			if (IsNeedUpdate())
			{
				Prepare();
				UpdateParticles();
				ApplyParticlesToTransforms();
			}
		}
		m_PreUpdateCount = 0;
	}

	private void Prepare()
	{
		m_DeltaTime = Time.deltaTime;
		if (m_UpdateMode == UpdateMode.UnscaledTime)
		{
			m_DeltaTime = Time.unscaledDeltaTime;
		}
		else if (m_UpdateMode == UpdateMode.AnimatePhysics)
		{
			m_DeltaTime = Time.fixedDeltaTime * (float)m_PreUpdateCount;
		}
		m_ObjectScale = Mathf.Abs(base.transform.lossyScale.x);
		m_ObjectMove = base.transform.position - m_ObjectPrevPosition;
		m_ObjectPrevPosition = base.transform.position;
		for (int i = 0; i < m_ParticleTrees.Count; i++)
		{
			ParticleTree particleTree = m_ParticleTrees[i];
			particleTree.m_RestGravity = particleTree.m_Root.TransformDirection(particleTree.m_LocalGravity);
			for (int j = 0; j < particleTree.m_Particles.Count; j++)
			{
				Particle particle = particleTree.m_Particles[j];
				if (particle.m_TransformNotNull)
				{
					particle.m_TransformPosition = particle.m_Transform.position;
					particle.m_TransformLocalPosition = particle.m_Transform.localPosition;
					particle.m_TransformLocalToWorldMatrix = particle.m_Transform.localToWorldMatrix;
				}
			}
		}
		if (m_EffectiveColliders != null)
		{
			m_EffectiveColliders.Clear();
		}
		if (m_Colliders == null)
		{
			return;
		}
		for (int k = 0; k < m_Colliders.Count; k++)
		{
			DynamicBoneColliderBase dynamicBoneColliderBase = m_Colliders[k];
			if (dynamicBoneColliderBase != null && dynamicBoneColliderBase.enabled)
			{
				if (m_EffectiveColliders == null)
				{
					m_EffectiveColliders = new List<DynamicBoneColliderBase>();
				}
				m_EffectiveColliders.Add(dynamicBoneColliderBase);
				if (dynamicBoneColliderBase.PrepareFrame != s_PrepareFrame)
				{
					dynamicBoneColliderBase.Prepare();
					dynamicBoneColliderBase.PrepareFrame = s_PrepareFrame;
				}
			}
		}
	}

	private bool IsNeedUpdate()
	{
		if (m_Weight > 0f)
		{
			if (m_DistantDisable)
			{
				return !m_DistantDisabled;
			}
			return true;
		}
		return false;
	}

	private void PreUpdate()
	{
		if (IsNeedUpdate())
		{
			InitTransforms();
		}
		m_PreUpdateCount++;
	}

	private void CheckDistance()
	{
		if (!m_DistantDisable)
		{
			return;
		}
		Transform referenceObject = m_ReferenceObject;
		if (referenceObject == null && Camera.main != null)
		{
			referenceObject = Camera.main.transform;
		}
		if (!(referenceObject != null))
		{
			return;
		}
		bool flag = (referenceObject.position - base.transform.position).sqrMagnitude > m_DistanceToObject * m_DistanceToObject;
		if (flag != m_DistantDisabled)
		{
			if (!flag)
			{
				ResetParticlesPosition();
			}
			m_DistantDisabled = flag;
		}
	}

	private void OnEnable()
	{
		ResetParticlesPosition();
	}

	private void OnDisable()
	{
		InitTransforms();
	}

	private void OnValidate()
	{
		m_UpdateRate = Mathf.Max(m_UpdateRate, 0f);
		m_Damping = Mathf.Clamp01(m_Damping);
		m_Elasticity = Mathf.Clamp01(m_Elasticity);
		m_Stiffness = Mathf.Clamp01(m_Stiffness);
		m_Inert = Mathf.Clamp01(m_Inert);
		m_Friction = Mathf.Clamp01(m_Friction);
		m_Radius = Mathf.Max(m_Radius, 0f);
		if (Application.isEditor && Application.isPlaying)
		{
			if (IsRootChanged())
			{
				InitTransforms();
				SetupParticles();
			}
			else
			{
				UpdateParameters();
			}
		}
	}

	private bool IsRootChanged()
	{
		List<Transform> list = new List<Transform>();
		if (m_Root != null)
		{
			list.Add(m_Root);
		}
		if (m_Roots != null)
		{
			foreach (Transform root in m_Roots)
			{
				if (root != null && !list.Contains(root))
				{
					list.Add(root);
				}
			}
		}
		if (list.Count != m_ParticleTrees.Count)
		{
			return true;
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] != m_ParticleTrees[i].m_Root)
			{
				return true;
			}
		}
		return false;
	}

	private void OnDidApplyAnimationProperties()
	{
		UpdateParameters();
	}

	private void OnDrawGizmosSelected()
	{
		if (base.enabled)
		{
			if (Application.isEditor && !Application.isPlaying && base.transform.hasChanged)
			{
				SetupParticles();
			}
			Gizmos.color = Color.white;
			for (int i = 0; i < m_ParticleTrees.Count; i++)
			{
				DrawGizmos(m_ParticleTrees[i]);
			}
		}
	}

	private void DrawGizmos(ParticleTree pt)
	{
		for (int i = 0; i < pt.m_Particles.Count; i++)
		{
			Particle particle = pt.m_Particles[i];
			if (particle.m_ParentIndex >= 0)
			{
				Particle particle2 = pt.m_Particles[particle.m_ParentIndex];
				Gizmos.DrawLine(particle.m_Position, particle2.m_Position);
			}
			if (particle.m_Radius > 0f)
			{
				Gizmos.DrawWireSphere(particle.m_Position, particle.m_Radius * m_ObjectScale);
			}
		}
	}

	public void SetWeight(float w)
	{
		if (m_Weight != w)
		{
			if (w == 0f)
			{
				InitTransforms();
			}
			else if (m_Weight == 0f)
			{
				ResetParticlesPosition();
			}
			m_Weight = (m_BlendWeight = w);
		}
	}

	public float GetWeight()
	{
		return m_Weight;
	}

	private void UpdateParticles()
	{
		if (m_ParticleTrees.Count <= 0)
		{
			return;
		}
		int num = 1;
		float timeVar = 1f;
		float deltaTime = m_DeltaTime;
		if (m_UpdateMode == UpdateMode.Default)
		{
			if (m_UpdateRate > 0f)
			{
				timeVar = deltaTime * m_UpdateRate;
			}
		}
		else if (m_UpdateRate > 0f)
		{
			float num2 = 1f / m_UpdateRate;
			m_Time += deltaTime;
			num = 0;
			while (m_Time >= num2)
			{
				m_Time -= num2;
				if (++num >= 3)
				{
					m_Time = 0f;
					break;
				}
			}
		}
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				UpdateParticles1(timeVar, i);
				UpdateParticles2(timeVar);
			}
		}
		else
		{
			SkipUpdateParticles();
		}
	}

	public void SetupParticles()
	{
		m_ParticleTrees.Clear();
		if (m_Root != null)
		{
			AppendParticleTree(m_Root);
		}
		if (m_Roots != null)
		{
			for (int i = 0; i < m_Roots.Count; i++)
			{
				Transform root = m_Roots[i];
				if (!(root == null) && !m_ParticleTrees.Exists((ParticleTree x) => x.m_Root == root))
				{
					AppendParticleTree(root);
				}
			}
		}
		m_ObjectScale = Mathf.Abs(base.transform.lossyScale.x);
		m_ObjectPrevPosition = base.transform.position;
		m_ObjectMove = Vector3.zero;
		for (int j = 0; j < m_ParticleTrees.Count; j++)
		{
			ParticleTree particleTree = m_ParticleTrees[j];
			AppendParticles(particleTree, particleTree.m_Root, -1, 0f);
		}
		UpdateParameters();
	}

	private void AppendParticleTree(Transform root)
	{
		if (!(root == null))
		{
			ParticleTree particleTree = new ParticleTree();
			particleTree.m_Root = root;
			particleTree.m_RootWorldToLocalMatrix = root.worldToLocalMatrix;
			m_ParticleTrees.Add(particleTree);
		}
	}

	private void AppendParticles(ParticleTree pt, Transform b, int parentIndex, float boneLength)
	{
		Particle particle = new Particle();
		particle.m_Transform = b;
		particle.m_TransformNotNull = b != null;
		particle.m_ParentIndex = parentIndex;
		if (b != null)
		{
			particle.m_Position = (particle.m_PrevPosition = b.position);
			particle.m_InitLocalPosition = b.localPosition;
			particle.m_InitLocalRotation = b.localRotation;
		}
		else
		{
			Transform transform = pt.m_Particles[parentIndex].m_Transform;
			if (m_EndLength > 0f)
			{
				Transform parent = transform.parent;
				if (parent != null)
				{
					particle.m_EndOffset = transform.InverseTransformPoint(transform.position * 2f - parent.position) * m_EndLength;
				}
				else
				{
					particle.m_EndOffset = new Vector3(m_EndLength, 0f, 0f);
				}
			}
			else
			{
				particle.m_EndOffset = transform.InverseTransformPoint(base.transform.TransformDirection(m_EndOffset) + transform.position);
			}
			particle.m_Position = (particle.m_PrevPosition = transform.TransformPoint(particle.m_EndOffset));
			particle.m_InitLocalPosition = Vector3.zero;
			particle.m_InitLocalRotation = Quaternion.identity;
		}
		if (parentIndex >= 0)
		{
			boneLength += (pt.m_Particles[parentIndex].m_Transform.position - particle.m_Position).magnitude;
			particle.m_BoneLength = boneLength;
			pt.m_BoneTotalLength = Mathf.Max(pt.m_BoneTotalLength, boneLength);
			pt.m_Particles[parentIndex].m_ChildCount++;
		}
		int count = pt.m_Particles.Count;
		pt.m_Particles.Add(particle);
		if (!(b != null))
		{
			return;
		}
		for (int i = 0; i < b.childCount; i++)
		{
			Transform child = b.GetChild(i);
			bool flag = false;
			if (m_Exclusions != null)
			{
				flag = m_Exclusions.Contains(child);
			}
			if (!flag)
			{
				AppendParticles(pt, child, count, boneLength);
			}
			else if (m_EndLength > 0f || m_EndOffset != Vector3.zero)
			{
				AppendParticles(pt, null, count, boneLength);
			}
		}
		if (b.childCount == 0 && (m_EndLength > 0f || m_EndOffset != Vector3.zero))
		{
			AppendParticles(pt, null, count, boneLength);
		}
	}

	public void UpdateParameters()
	{
		SetWeight(m_BlendWeight);
		for (int i = 0; i < m_ParticleTrees.Count; i++)
		{
			UpdateParameters(m_ParticleTrees[i]);
		}
	}

	private void UpdateParameters(ParticleTree pt)
	{
		pt.m_LocalGravity = pt.m_RootWorldToLocalMatrix.MultiplyVector(m_Gravity).normalized * m_Gravity.magnitude;
		for (int i = 0; i < pt.m_Particles.Count; i++)
		{
			Particle particle = pt.m_Particles[i];
			particle.m_Damping = m_Damping;
			particle.m_Elasticity = m_Elasticity;
			particle.m_Stiffness = m_Stiffness;
			particle.m_Inert = m_Inert;
			particle.m_Friction = m_Friction;
			particle.m_Radius = m_Radius;
			if (pt.m_BoneTotalLength > 0f)
			{
				float time = particle.m_BoneLength / pt.m_BoneTotalLength;
				if (m_DampingDistrib != null && m_DampingDistrib.keys.Length != 0)
				{
					particle.m_Damping *= m_DampingDistrib.Evaluate(time);
				}
				if (m_ElasticityDistrib != null && m_ElasticityDistrib.keys.Length != 0)
				{
					particle.m_Elasticity *= m_ElasticityDistrib.Evaluate(time);
				}
				if (m_StiffnessDistrib != null && m_StiffnessDistrib.keys.Length != 0)
				{
					particle.m_Stiffness *= m_StiffnessDistrib.Evaluate(time);
				}
				if (m_InertDistrib != null && m_InertDistrib.keys.Length != 0)
				{
					particle.m_Inert *= m_InertDistrib.Evaluate(time);
				}
				if (m_FrictionDistrib != null && m_FrictionDistrib.keys.Length != 0)
				{
					particle.m_Friction *= m_FrictionDistrib.Evaluate(time);
				}
				if (m_RadiusDistrib != null && m_RadiusDistrib.keys.Length != 0)
				{
					particle.m_Radius *= m_RadiusDistrib.Evaluate(time);
				}
			}
			particle.m_Damping = Mathf.Clamp01(particle.m_Damping);
			particle.m_Elasticity = Mathf.Clamp01(particle.m_Elasticity);
			particle.m_Stiffness = Mathf.Clamp01(particle.m_Stiffness);
			particle.m_Inert = Mathf.Clamp01(particle.m_Inert);
			particle.m_Friction = Mathf.Clamp01(particle.m_Friction);
			particle.m_Radius = Mathf.Max(particle.m_Radius, 0f);
		}
	}

	private void InitTransforms()
	{
		for (int i = 0; i < m_ParticleTrees.Count; i++)
		{
			InitTransforms(m_ParticleTrees[i]);
		}
	}

	private void InitTransforms(ParticleTree pt)
	{
		for (int i = 0; i < pt.m_Particles.Count; i++)
		{
			Particle particle = pt.m_Particles[i];
			if (particle.m_TransformNotNull)
			{
				particle.m_Transform.localPosition = particle.m_InitLocalPosition;
				particle.m_Transform.localRotation = particle.m_InitLocalRotation;
			}
		}
	}

	private void ResetParticlesPosition()
	{
		for (int i = 0; i < m_ParticleTrees.Count; i++)
		{
			ResetParticlesPosition(m_ParticleTrees[i]);
		}
		m_ObjectPrevPosition = base.transform.position;
	}

	private void ResetParticlesPosition(ParticleTree pt)
	{
		for (int i = 0; i < pt.m_Particles.Count; i++)
		{
			Particle particle = pt.m_Particles[i];
			if (particle.m_TransformNotNull)
			{
				particle.m_Position = (particle.m_PrevPosition = particle.m_Transform.position);
			}
			else
			{
				Transform transform = pt.m_Particles[particle.m_ParentIndex].m_Transform;
				particle.m_Position = (particle.m_PrevPosition = transform.TransformPoint(particle.m_EndOffset));
			}
			particle.m_isCollide = false;
		}
	}

	private void UpdateParticles1(float timeVar, int loopIndex)
	{
		for (int i = 0; i < m_ParticleTrees.Count; i++)
		{
			UpdateParticles1(m_ParticleTrees[i], timeVar, loopIndex);
		}
	}

	private void UpdateParticles1(ParticleTree pt, float timeVar, int loopIndex)
	{
		Vector3 gravity = m_Gravity;
		Vector3 normalized = m_Gravity.normalized;
		Vector3 vector = normalized * Mathf.Max(Vector3.Dot(pt.m_RestGravity, normalized), 0f);
		gravity -= vector;
		gravity = (gravity + m_Force) * (m_ObjectScale * timeVar);
		Vector3 vector2 = ((loopIndex == 0) ? m_ObjectMove : Vector3.zero);
		for (int i = 0; i < pt.m_Particles.Count; i++)
		{
			Particle particle = pt.m_Particles[i];
			if (particle.m_ParentIndex >= 0)
			{
				Vector3 vector3 = particle.m_Position - particle.m_PrevPosition;
				Vector3 vector4 = vector2 * particle.m_Inert;
				particle.m_PrevPosition = particle.m_Position + vector4;
				float num = particle.m_Damping;
				if (particle.m_isCollide)
				{
					num += particle.m_Friction;
					if (num > 1f)
					{
						num = 1f;
					}
					particle.m_isCollide = false;
				}
				particle.m_Position += vector3 * (1f - num) + gravity + vector4;
			}
			else
			{
				particle.m_PrevPosition = particle.m_Position;
				particle.m_Position = particle.m_TransformPosition;
			}
		}
	}

	private void UpdateParticles2(float timeVar)
	{
		for (int i = 0; i < m_ParticleTrees.Count; i++)
		{
			UpdateParticles2(m_ParticleTrees[i], timeVar);
		}
	}

	private void UpdateParticles2(ParticleTree pt, float timeVar)
	{
		Plane plane = default(Plane);
		for (int i = 1; i < pt.m_Particles.Count; i++)
		{
			Particle particle = pt.m_Particles[i];
			Particle particle2 = pt.m_Particles[particle.m_ParentIndex];
			float num = ((!particle.m_TransformNotNull) ? particle2.m_TransformLocalToWorldMatrix.MultiplyVector(particle.m_EndOffset).magnitude : (particle2.m_TransformPosition - particle.m_TransformPosition).magnitude);
			float num2 = Mathf.Lerp(1f, particle.m_Stiffness, m_Weight);
			if (num2 > 0f || particle.m_Elasticity > 0f)
			{
				Matrix4x4 transformLocalToWorldMatrix = particle2.m_TransformLocalToWorldMatrix;
				transformLocalToWorldMatrix.SetColumn(3, particle2.m_Position);
				Vector3 vector = ((!particle.m_TransformNotNull) ? transformLocalToWorldMatrix.MultiplyPoint3x4(particle.m_EndOffset) : transformLocalToWorldMatrix.MultiplyPoint3x4(particle.m_TransformLocalPosition));
				Vector3 vector2 = vector - particle.m_Position;
				particle.m_Position += vector2 * (particle.m_Elasticity * timeVar);
				if (num2 > 0f)
				{
					vector2 = vector - particle.m_Position;
					float magnitude = vector2.magnitude;
					float num3 = num * (1f - num2) * 2f;
					if (magnitude > num3)
					{
						particle.m_Position += vector2 * ((magnitude - num3) / magnitude);
					}
				}
			}
			if (m_EffectiveColliders != null)
			{
				float particleRadius = particle.m_Radius * m_ObjectScale;
				for (int j = 0; j < m_EffectiveColliders.Count; j++)
				{
					DynamicBoneColliderBase dynamicBoneColliderBase = m_EffectiveColliders[j];
					particle.m_isCollide |= dynamicBoneColliderBase.Collide(ref particle.m_Position, particleRadius);
				}
			}
			if (m_FreezeAxis != 0)
			{
				Vector3 inNormal = particle2.m_TransformLocalToWorldMatrix.GetColumn((int)(m_FreezeAxis - 1)).normalized;
				plane.SetNormalAndPosition(inNormal, particle2.m_Position);
				particle.m_Position -= plane.normal * plane.GetDistanceToPoint(particle.m_Position);
			}
			Vector3 vector3 = particle2.m_Position - particle.m_Position;
			float magnitude2 = vector3.magnitude;
			if (magnitude2 > 0f)
			{
				particle.m_Position += vector3 * ((magnitude2 - num) / magnitude2);
			}
		}
	}

	private void SkipUpdateParticles()
	{
		for (int i = 0; i < m_ParticleTrees.Count; i++)
		{
			SkipUpdateParticles(m_ParticleTrees[i]);
		}
	}

	private void SkipUpdateParticles(ParticleTree pt)
	{
		for (int i = 0; i < pt.m_Particles.Count; i++)
		{
			Particle particle = pt.m_Particles[i];
			if (particle.m_ParentIndex >= 0)
			{
				particle.m_PrevPosition += m_ObjectMove;
				particle.m_Position += m_ObjectMove;
				Particle particle2 = pt.m_Particles[particle.m_ParentIndex];
				float num = ((!particle.m_TransformNotNull) ? particle2.m_TransformLocalToWorldMatrix.MultiplyVector(particle.m_EndOffset).magnitude : (particle2.m_TransformPosition - particle.m_TransformPosition).magnitude);
				float num2 = Mathf.Lerp(1f, particle.m_Stiffness, m_Weight);
				if (num2 > 0f)
				{
					Matrix4x4 transformLocalToWorldMatrix = particle2.m_TransformLocalToWorldMatrix;
					transformLocalToWorldMatrix.SetColumn(3, particle2.m_Position);
					Vector3 vector = ((!particle.m_TransformNotNull) ? transformLocalToWorldMatrix.MultiplyPoint3x4(particle.m_EndOffset) : transformLocalToWorldMatrix.MultiplyPoint3x4(particle.m_TransformLocalPosition));
					Vector3 vector2 = vector - particle.m_Position;
					float magnitude = vector2.magnitude;
					float num3 = num * (1f - num2) * 2f;
					if (magnitude > num3)
					{
						particle.m_Position += vector2 * ((magnitude - num3) / magnitude);
					}
				}
				Vector3 vector3 = particle2.m_Position - particle.m_Position;
				float magnitude2 = vector3.magnitude;
				if (magnitude2 > 0f)
				{
					particle.m_Position += vector3 * ((magnitude2 - num) / magnitude2);
				}
			}
			else
			{
				particle.m_PrevPosition = particle.m_Position;
				particle.m_Position = particle.m_TransformPosition;
			}
		}
	}

	private static Vector3 MirrorVector(Vector3 v, Vector3 axis)
	{
		return v - axis * (Vector3.Dot(v, axis) * 2f);
	}

	private void ApplyParticlesToTransforms()
	{
		Vector3 right = Vector3.right;
		Vector3 up = Vector3.up;
		Vector3 forward = Vector3.forward;
		bool nx = false;
		bool ny = false;
		bool nz = false;
		for (int i = 0; i < m_ParticleTrees.Count; i++)
		{
			ApplyParticlesToTransforms(m_ParticleTrees[i], right, up, forward, nx, ny, nz);
		}
	}

	private void ApplyParticlesToTransforms(ParticleTree pt, Vector3 ax, Vector3 ay, Vector3 az, bool nx, bool ny, bool nz)
	{
		for (int i = 1; i < pt.m_Particles.Count; i++)
		{
			Particle particle = pt.m_Particles[i];
			Particle particle2 = pt.m_Particles[particle.m_ParentIndex];
			if (particle2.m_ChildCount <= 1)
			{
				Vector3 direction = ((!particle.m_TransformNotNull) ? particle.m_EndOffset : particle.m_Transform.localPosition);
				Vector3 fromDirection = particle2.m_Transform.TransformDirection(direction);
				Vector3 toDirection = particle.m_Position - particle2.m_Position;
				Quaternion quaternion = Quaternion.FromToRotation(fromDirection, toDirection);
				particle2.m_Transform.rotation = quaternion * particle2.m_Transform.rotation;
			}
			if (particle.m_TransformNotNull)
			{
				particle.m_Transform.position = particle.m_Position;
			}
		}
	}

	private static void AddPendingWork(DynamicBone db)
	{
		s_PendingWorks.Add(db);
	}

	private static void AddWorkToQueue(DynamicBone db)
	{
		s_WorkQueueSemaphore.Release();
	}

	private static DynamicBone GetWorkFromQueue()
	{
		int index = Interlocked.Increment(ref s_WorkQueueIndex);
		return s_EffectiveWorks[index];
	}

	private static void ThreadProc()
	{
		while (true)
		{
			s_WorkQueueSemaphore.WaitOne();
			GetWorkFromQueue().UpdateParticles();
			if (Interlocked.Decrement(ref s_RemainWorkCount) <= 0)
			{
				s_AllWorksDoneEvent.Set();
			}
		}
	}

	private static void InitThreadPool()
	{
		s_AllWorksDoneEvent = new AutoResetEvent(initialState: false);
		s_WorkQueueSemaphore = new Semaphore(0, int.MaxValue);
		int processorCount = Environment.ProcessorCount;
		for (int i = 0; i < processorCount; i++)
		{
			Thread thread = new Thread(ThreadProc);
			thread.IsBackground = true;
			thread.Start();
		}
	}

	private static void ExecuteWorks()
	{
		if (s_PendingWorks.Count <= 0)
		{
			return;
		}
		s_EffectiveWorks.Clear();
		for (int i = 0; i < s_PendingWorks.Count; i++)
		{
			DynamicBone dynamicBone = s_PendingWorks[i];
			if (dynamicBone != null && dynamicBone.enabled)
			{
				dynamicBone.CheckDistance();
				if (dynamicBone.IsNeedUpdate())
				{
					s_EffectiveWorks.Add(dynamicBone);
				}
			}
		}
		s_PendingWorks.Clear();
		if (s_EffectiveWorks.Count > 0)
		{
			if (s_AllWorksDoneEvent == null)
			{
				InitThreadPool();
			}
			int num = (s_RemainWorkCount = s_EffectiveWorks.Count);
			s_WorkQueueIndex = -1;
			for (int j = 0; j < num; j++)
			{
				DynamicBone dynamicBone2 = s_EffectiveWorks[j];
				dynamicBone2.Prepare();
				AddWorkToQueue(dynamicBone2);
			}
			s_AllWorksDoneEvent.WaitOne();
			for (int k = 0; k < num; k++)
			{
				s_EffectiveWorks[k].ApplyParticlesToTransforms();
			}
		}
	}
}
