using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedObjActivation : MonoBehaviour
{
	[Header("Framework")]
	public List<ObjectParams> Objects;

	private void Start()
	{
		if (Objects == null)
		{
			return;
		}
		foreach (ObjectParams @object in Objects)
		{
			StartCoroutine(Activate(@object.Object, @object.OptionalMessage, @object.Mode.ToString(), @object.Timer, @object.SwitchTimer, @object.OptionalSwitchMessage, @object.NewParent, @object.OptionalParentMessage));
		}
	}

	private void Update()
	{
		if (Objects.Count == 0)
		{
			return;
		}
		for (int i = 0; i < Objects.Count; i++)
		{
			if (Objects[i].Object == null)
			{
				Objects.RemoveAt(i);
			}
		}
	}

	private IEnumerator Activate(GameObject Obj, string Message, string Mode, float TimeToExec, float TimeToSwitch, string SwitchMessage, Transform ToParent, string ParentMessage)
	{
		yield return new WaitForSeconds(TimeToExec);
		if (Message != "" && Mode == "Disable")
		{
			Obj.SendMessage(Message, 0, SendMessageOptions.DontRequireReceiver);
		}
		if (Mode != "Switch")
		{
			Obj.SetActive((Mode == "Enable") ? true : false);
		}
		else
		{
			Obj.SetActive(!Obj.activeSelf);
			StartCoroutine(SwitchObject(Obj, TimeToSwitch, SwitchMessage));
		}
		if (Message != "" && Mode == "Enable")
		{
			Obj.SendMessage(Message, 0, SendMessageOptions.DontRequireReceiver);
		}
		if ((bool)ToParent)
		{
			Obj.transform.SetParent(ToParent);
			if (ParentMessage != "")
			{
				ToParent.SendMessage(ParentMessage, 0, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	private IEnumerator SwitchObject(GameObject Obj, float TimeToExec, string SwitchMessage)
	{
		yield return new WaitForSeconds(TimeToExec);
		if (SwitchMessage != "")
		{
			Obj.SendMessage(SwitchMessage, 0, SendMessageOptions.DontRequireReceiver);
		}
		if ((bool)Obj)
		{
			Obj.SetActive(!Obj.activeSelf);
		}
	}
}
