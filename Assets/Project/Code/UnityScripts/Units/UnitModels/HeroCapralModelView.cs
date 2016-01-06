using UnityEngine;
using System.Collections;

public class HeroCapralModelView : UnitModelView {
	[SerializeField]
	private float _clipDischargeStanceOffset = 0f;

	public new void Awake() {
		base.Awake();

		_animationClipName.Add(EUnitAnimationState.Skill_ClipDischarge, "Skill_ClipDischarge");
		_animationClipName.Add(EUnitAnimationState.Skill_ExplosiveCharges, "Skill_ExplosiveCharges");
		_animationClipName.Add(EUnitAnimationState.Skill_StunGrenade, "Skill_StunGrenade");
		_animationClipName.Add(EUnitAnimationState.Win, "Waiting");
		_animationClipName.Add(EUnitAnimationState.Speak_1, "Speak");

		_hitAnimations[1] = _hitAnimations[2] = _hitAnimations[3] = "GetDamage_1";

		_animDeath = EUnitAnimationState.Death_FallForward;
	}

	public override void SetWeaponType(EItemKey weaponRKey, EItemKey weaponLKey) {
		_animRun = EUnitAnimationState.Run_Gun;
		_animAttack = EUnitAnimationState.Strike_Gun;
		_weaponStanceOffset = _gunStanceOffset;
	}

	public override void PlayHitAnimation(int totalHealth, HitInfo hitInfo) {
		if (_animator.GetCurrentAnimatorStateInfo(0).IsName(_animationClipName[EUnitAnimationState.Skill_ClipDischarge]) ||
			_animator.GetCurrentAnimatorStateInfo(0).IsName(_animationClipName[EUnitAnimationState.Skill_StunGrenade])) {
				return;
		}

		base.PlayHitAnimation(totalHealth, hitInfo);
	}

	#region animations
	public override void PlaySkillAnimation(ESkillKey skillKey, params object[] data) {
		switch (skillKey) {
			case ESkillKey.ClipDischarge:
				PlaySkillClipDischargeAnimation((float)data[0]);
				break;
			case ESkillKey.ExplosiveCharges:
				PlaySkillExplosiveChargesAnimation();
				break;
			case ESkillKey.StunGrenade:
				PlaySkillStunGrenadeAnimation();
				break;
		}
	}

	private void PlaySkillClipDischargeAnimation(float distanceToTarget) {
		distanceToTarget -= _clipDischargeStanceOffset;

		_animator.Play(_animationClipName[EUnitAnimationState.Skill_ClipDischarge], 0, 0f);
		/*if (_weaponViewRH != null) {
			_weaponViewRH.PlayShot(distanceToTarget);
		}
		if (_weaponViewLH != null) {
			_weaponViewLH.PlayShot(distanceToTarget);
		}*/
	}

	private void PlaySkillExplosiveChargesAnimation() {
		_animator.Play(_animationClipName[EUnitAnimationState.Skill_ExplosiveCharges], 0, 0f);
	}

	private void PlaySkillStunGrenadeAnimation() {
		_animator.Play(_animationClipName[EUnitAnimationState.Skill_StunGrenade], 0, 0f);
	}

	public override void PlayWinAnimation() {
		_animator.Play(_animationClipName[EUnitAnimationState.Win], 0, 0f);
	}

	public override void PlaySpeakAnimation() {
		_animator.speed = 1f;
		_animator.Play(_animationClipName[EUnitAnimationState.Speak_1]);
	}
	#endregion
}
