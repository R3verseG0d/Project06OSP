public class MetalSonicEffects : EffectsBase
{
	public override void Update()
	{
		base.Update();
		UpdateJumpBallFX(PM.Base.GetState() == "Jump" && PM.Base.JumpAnimation == 1);
	}
}
