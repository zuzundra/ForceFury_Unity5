using UnityEngine;
using System.Collections;

public class SoldierScoutModelView : UnitModelView {
	public new void Awake() {
		base.Awake();

		_animationClipName.Add(EUnitAnimationState.Win, "Waiting");
		_animationClipName.Add(EUnitAnimationState.Condition_Stun, "Condition_Stun");

		_hitAnimations[1] = _hitAnimations[2] = _hitAnimations[3] = "GetDamage_1";

		_animDeath = EUnitAnimationState.Death_FallBack;
	}

	public override void SetWeaponType(EItemKey weaponRKey, EItemKey weaponLKey) {
		_animRun = EUnitAnimationState.Run_Gun;
		_animAttack = EUnitAnimationState.Strike_Gun;
		_weaponStanceOffset = _gunStanceOffset;
	}

	#region animations
	public override void PlayWinAnimation() {
		_animator.Play(_animationClipName[EUnitAnimationState.Win], 0, 0f);
	}

	public override void PlayStunAnimation() {
		_animator.Play(_animationClipName[EUnitAnimationState.Condition_Stun], 0, 0f);
	}
	#endregion
}