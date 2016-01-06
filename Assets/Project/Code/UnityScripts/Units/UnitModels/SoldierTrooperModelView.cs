using UnityEngine;
using System.Collections;

public class SoldierTrooperModelView : UnitModelView {
	public new void Awake() {
		base.Awake();

		_animationClipName.Add(EUnitAnimationState.Win, "Waiting");

		_hitAnimations[1] = _hitAnimations[2] = _hitAnimations[3] = "GetDamage_1";

		_animDeath = EUnitAnimationState.Death_FallBack;
	}

	public override void SetWeaponType(EItemKey weaponRKey, EItemKey weaponLKey) {
		_animRun = EUnitAnimationState.Run_Rifle;
		_animAttack = EUnitAnimationState.Strike_Rifle;
		_weaponStanceOffset = _rifleStanceOffset;
	}

	#region animations
	public override void PlayWinAnimation() {
		_animator.Play(_animationClipName[EUnitAnimationState.Win], 0, 0f);
	}
	#endregion
}