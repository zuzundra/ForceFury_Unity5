using UnityEngine;
using System.Collections;

public class SkillClipDischargeView : MonoBehaviour {
	private Vector3 _lPos;
	private Vector3 _rPos;

	public void StoreWeaponPosition(BaseUnitBehaviour caster) {
		_lPos = caster.ModelView.WeaponLeft != null ? caster.ModelView.WeaponLeft.GunfireParticleParent.position : Vector3.zero;
		_rPos = caster.ModelView.WeaponRight != null ? caster.ModelView.WeaponRight.GunfireParticleParent.position : Vector3.zero;
	}

	public void Play(BaseUnitBehaviour caster) {
		caster.ModelView.DisplayWeaponShot(caster.DistanceToTarget, _lPos, _rPos);
	}
}
