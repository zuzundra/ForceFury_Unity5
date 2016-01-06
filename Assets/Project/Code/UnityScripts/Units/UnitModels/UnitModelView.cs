using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public class UnitModelView : MonoBehaviour {
	[SerializeField]
	protected SkinnedMeshRenderer _headArmorMeshRenderer;
	[SerializeField]
	protected SkinnedMeshRenderer _bodyArmorMeshRenderer;

	[SerializeField]
	protected Transform _weaponBoneRight;
	public Transform WeaponBoneRight {
		get { return _weaponBoneRight; }
	}

	[SerializeField]
	protected Transform _weaponBoneLeft;
	public Transform WeaponBoneLeft {
		get { return _weaponBoneLeft; }
	}

	[SerializeField]
	protected Animator _animator;
	public Animator Animator {
		get { return _animator; }
	}

	[SerializeField]
	protected float _shootPositionTimeOffset = 0f;
	public float ShootPositionTimeOffset {
		get { return _shootPositionTimeOffset; }
	}
	private WaitForSeconds _wfsAttackDelay;
	public WaitForSeconds WFSAttackDelay {
		get { return _wfsAttackDelay; }
	}

	[SerializeField]
	protected float _gunStanceOffset = 0.44f;
	[SerializeField]
	protected float _rifleStanceOffset = 0.13f;
	protected float _weaponStanceOffset = 0f;

	protected WeaponView _weaponViewRH = null;
	public WeaponView WeaponRight {
		get { return _weaponViewRH; }
	}

	protected WeaponView _weaponViewLH = null;
	public WeaponView WeaponLeft {
		get { return _weaponViewLH; }
	}

	protected EUnitAnimationState _animRun = EUnitAnimationState.Run_Gun;
	protected EUnitAnimationState _animAttack = EUnitAnimationState.Strike_Gun;
	protected EUnitAnimationState _animDeath = EUnitAnimationState.Death_FallForward;

	protected Dictionary<EUnitAnimationState, string> _animationClipName = new Dictionary<EUnitAnimationState, string>() {
		{ EUnitAnimationState.Idle, "Idle" },
		{ EUnitAnimationState.Run_Gun, "Run_Gun" },
		{ EUnitAnimationState.Run_Rifle, "Run_Rifle" },
		{ EUnitAnimationState.GetDamage_1, "GetDamage_1" },
		{ EUnitAnimationState.GetDamage_2, "GetDamage_2" },
		{ EUnitAnimationState.Strike_Gun, "Strike_Gun" },
		{ EUnitAnimationState.Strike_Rifle, "Strike_Rifle" },
		{ EUnitAnimationState.Death_FallForward, "Death_FallForward" },
		{ EUnitAnimationState.Death_FallBack, "Death_FallBack" }
	};

	protected Dictionary<EUnitAnimationState, int> _mainAnimationState = new Dictionary<EUnitAnimationState, int>() {
		{ EUnitAnimationState.Run_Gun, 1 },
		{ EUnitAnimationState.Run_Rifle, 2 },
		{ EUnitAnimationState.Strike_Gun, 11 },
		{ EUnitAnimationState.Strike_Rifle, 12 },
		{ EUnitAnimationState.Death_FallForward, 31 },
		{ EUnitAnimationState.Death_FallBack, 32 }
	};

	protected Dictionary<int, string> _hitAnimations = new Dictionary<int, string>() {
		{ 1, "GetDamage_1" },
		{ 2, "GetDamage_2" },
		{ 3, "GetDamage_1" },
	};

	protected float _runAnimationSpeed = 1f;	//how fast movement animation will play
	protected float _attackAnimationSpeed = 1f;	//how fast attack animation will play
	//protected float _hitAnimationSpeed = 1f;	//how fast hit animation will play
	protected float _deathAnimationSpeed = 1f;	//how death movement animation will play
	protected float _defaultMovementSpeed = 1.5f;	//how fast (in unity meters) unit will move with default animation speed
	public float MovementSpeed {
		set { /*_runAnimationSpeed = value / _defaultMovementSpeed;*/ }
	}

	private float _defaultModelScaleFactor = 2f;

	public float ModelHeight {
		get { return _bodyArmorMeshRenderer.bounds.size.y + _headArmorMeshRenderer.bounds.size.y; }
	}

	public void Awake() {
		if (_animator == null) {
			_animator = gameObject.GetComponent<Animator>();
		}

		_animDeath = (EUnitAnimationState)UnityEngine.Random.Range(_mainAnimationState[EUnitAnimationState.Death_FallForward], _mainAnimationState[EUnitAnimationState.Death_FallBack] + 1);

		if (_shootPositionTimeOffset > 0f) {
			_wfsAttackDelay = new WaitForSeconds(_shootPositionTimeOffset);
		}
	}

	public virtual void SetWeaponType(EItemKey weaponRKey, EItemKey weaponLKey) {
		if (weaponRKey != EItemKey.None && weaponLKey != EItemKey.None) {
			//TODO: setup dual animation
		} else {
			EItemType weaponType = ItemsConfig.Instance.GetItem(weaponRKey != EItemKey.None ? weaponRKey : weaponLKey).Type;
			switch (weaponType) {
				case EItemType.W_Gun:
					_animRun = EUnitAnimationState.Run_Gun;
					_animAttack = EUnitAnimationState.Strike_Gun;
					_weaponStanceOffset = _gunStanceOffset;
					break;
				case EItemType.W_Rifle:
					_animRun = EUnitAnimationState.Run_Rifle;
					_animAttack = EUnitAnimationState.Strike_Rifle;
					_weaponStanceOffset = _rifleStanceOffset;
					break;
			}
		}
	}

	public void SetupGraphics(GameObject rhWeaponResource, GameObject lhWeaponResource, GameObject headArmorResource, GameObject bodyArmorResource) {
		float scaleFix = transform.localScale.x / _defaultModelScaleFactor;

		//weapon
		if (_weaponBoneRight != null && rhWeaponResource != null) {
			GameObject weaponInstance = GameObject.Instantiate(rhWeaponResource) as GameObject;
			weaponInstance.transform.parent = _weaponBoneRight;
			weaponInstance.transform.localPosition = Vector3.zero;
			weaponInstance.transform.localRotation = Quaternion.identity;
			weaponInstance.transform.localScale *= scaleFix;

			_weaponViewRH = weaponInstance.GetComponent<WeaponView>();
		}
		if (_weaponBoneLeft != null && lhWeaponResource != null) {
			GameObject weaponInstance = GameObject.Instantiate(lhWeaponResource) as GameObject;
			weaponInstance.transform.parent = _weaponBoneLeft;
			weaponInstance.transform.localPosition = Vector3.zero;
			weaponInstance.transform.localRotation = Quaternion.identity;
			weaponInstance.transform.localScale *= scaleFix;

			_weaponViewLH = weaponInstance.GetComponent<WeaponView>();
		}

		//armor
		//TODO: maybe armor will need scale fixes
		if (_headArmorMeshRenderer != null && headArmorResource != null) {
			GameObject headArmorInstance = GameObject.Instantiate(headArmorResource) as GameObject;
			SwitchMesh(headArmorInstance.GetComponent<MeshFilter>().mesh, headArmorInstance.GetComponent<MeshRenderer>().material, _headArmorMeshRenderer);
			GameObject.Destroy(headArmorInstance);
		}
		if (_bodyArmorMeshRenderer != null && bodyArmorResource != null) {
			GameObject bodyArmorInstance = GameObject.Instantiate(bodyArmorResource) as GameObject;
			SwitchMesh(bodyArmorInstance.GetComponent<MeshFilter>().mesh, bodyArmorInstance.GetComponent<MeshRenderer>().material, _bodyArmorMeshRenderer);
			GameObject.Destroy(bodyArmorInstance);
		}
	}

	public void SetupWeapon() {
		if (_weaponViewRH != null) {
			_weaponViewRH.Setup(transform);
		}
		if (_weaponViewLH != null) {
			_weaponViewLH.Setup(transform);
		}
	}

	public void DisplayWeaponShot(float distanceToTarget, Vector3 leftWeaponPos, Vector3 rightWeaponPos) {
		if (_weaponViewRH != null) {
			_weaponViewRH.PlayShotFromPosition(distanceToTarget, rightWeaponPos);
		}
		if (_weaponViewLH != null) {
			_weaponViewLH.PlayShotFromPosition(distanceToTarget, leftWeaponPos);
		}
	}

	public void SimulateAttack() {
		_animator.speed = 0f;
		_animator.Play(_animationClipName[_animAttack], 0, _shootPositionTimeOffset);
	}

	public void UpdateProjectileColor(Color color) {
		if (_weaponViewLH != null) {
			_weaponViewLH.UpdateProjectileColor(color);
		}
		if (_weaponViewRH != null) {
			_weaponViewRH.UpdateProjectileColor(color);
		}
	}

	public void ResetProjectileColor() {
		if (_weaponViewLH != null) {
			_weaponViewLH.ResetProjectileColor();
		}
		if (_weaponViewRH != null) {
			_weaponViewRH.ResetProjectileColor();
		}
	}

	protected void SwitchMesh(Mesh sourceMesh, Material sourceMaterial, SkinnedMeshRenderer target) {
		target.sharedMesh = sourceMesh;
		target.material = sourceMaterial;

		target.sharedMesh.RecalculateBounds();
		target.sharedMesh.RecalculateNormals();
	}

	#region animations
	public virtual void StopCurrentAnimation() {
		if (_animator.GetInteger("MAS") == _mainAnimationState[_animAttack]) {
			if (_weaponViewRH != null) {
				_weaponViewRH.StopShot();
			}
			if (_weaponViewLH != null) {
				_weaponViewLH.StopShot();
			}
		}
		_animator.StopPlayback();
	}

	public virtual void StopAttackAnimation() {
		if (_animator.GetInteger("MAS") == _mainAnimationState[_animAttack]) {
			StopCurrentAnimation();
		}
	}

	public virtual void PlayIdleAnimation() {
		_animator.Play(_animationClipName[EUnitAnimationState.Idle]);
	}

	public virtual void PlaySkillAnimation(ESkillKey skillKey, params object[] data) { }

	public virtual void PlayRunAnimation() {
		_animator.speed = _runAnimationSpeed;
		_animator.Play(_animationClipName[_animRun]);
		_animator.SetInteger("MAS", _mainAnimationState[_animRun]);
	}

	public virtual void PlayHitAnimation(int totalHealth, HitInfo hitInfo) {
		float healthState1 = totalHealth * 0.75f;
		float healthState2 = totalHealth * 0.5f;
		float healthState3 = totalHealth * 0.25f;

		int hitAnimationState = 0;
		if (hitInfo.HealthBefore > healthState1 && hitInfo.HealthAfter < healthState1) {
			hitAnimationState = 1;
		} else if (hitInfo.HealthBefore > healthState3 && hitInfo.HealthAfter < healthState3) {
			hitAnimationState = 2;
		} else if (hitInfo.HealthBefore > healthState2 && hitInfo.HealthAfter < healthState2) {
			hitAnimationState = 3;
		}

		if(_hitAnimations.ContainsKey(hitAnimationState)) {
			_animator.CrossFade(_hitAnimations[hitAnimationState], 0.2f, 0, 0f);
		}

		//_animator.SetTrigger("GetDamage1"); //old way
	}

	public virtual void PlayAttackAnimation(float distanceToTarget) {
		_animator.speed = _attackAnimationSpeed;
		_animator.Play(_animationClipName[_animAttack], 0, 0f);
		_animator.SetInteger("MAS", _mainAnimationState[_animAttack]);

		StartCoroutine(PlayAttackAnimationInternal(distanceToTarget));
	}

	private IEnumerator PlayAttackAnimationInternal(float distanceToTarget) {
        if (_wfsAttackDelay != null)
        {
			yield return _wfsAttackDelay;
		}
		distanceToTarget -= _weaponStanceOffset;
		if (_weaponViewRH != null) {
			_weaponViewRH.PlayShot(distanceToTarget);
		}
		if (_weaponViewLH != null) {
			_weaponViewLH.PlayShot(distanceToTarget);
		}
	}

	public virtual void PlayDeathAnimation(Action onAnimationEnd) {
		_animator.speed = _deathAnimationSpeed;
		_animator.SetInteger("MAS", _mainAnimationState[_animDeath]);
		_animator.Play(_animationClipName[_animDeath]);

		StartCoroutine(DeathAnimationEndWaiter(onAnimationEnd));
	}

	protected IEnumerator DeathAnimationEndWaiter(Action onAnimationEnd) {
		yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
		onAnimationEnd();
	}

	public virtual void PlayWinAnimation() {
		//TODO: win animation
		_animator.speed = 0f;
		_animator.StopPlayback();
	}

	public virtual void PlaySpeakAnimation() { }

	public virtual void PlayStunAnimation() { }
	#endregion
}
