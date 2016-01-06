using System;
using System.Collections.Generic;

/// <summary>
/// Information about heroes player have
/// </summary>
public class PlayerHeroes {
	private BaseHero[] _heroes = null;
	private ArrayRO<BaseHero> _heroesRO = null;
	public ArrayRO<BaseHero> Heroes {
		get { return _heroesRO; }
	}

	public BaseHero Current {
		get { return _heroes != null && _currentHeroIndex >= 0 && _currentHeroIndex < _heroes.Length ? _heroes[_currentHeroIndex] : null; }
	}

	private int _currentHeroIndex = -1;

	public PlayerHeroes(BaseHero[] heroes, int currentHeroIndex) {
		AddHeroes(heroes);
		SelectHero(currentHeroIndex);
	}

	public void AddHeroes(BaseHero[] heroesList) {
		if(_heroes == null) {
			_heroes = heroesList;
		} else if(heroesList.Length > 0) {
			BaseHero[] newHeroes = new BaseHero[_heroes.Length + heroesList.Length];
			Array.Copy(_heroes, newHeroes, _heroes.Length);
			for (int i = _heroes.Length, j = 0; i < newHeroes.Length; i++, j++) {
				newHeroes[i] = heroesList[j];
			}
			_heroes = newHeroes;
		}

		_heroesRO = new ArrayRO<BaseHero>(_heroes);
	}

	public void SelectHero(BaseHero hero) {
		if (_heroes != null) {
			for (int i = 0; i < _heroes.Length; i++) {
				if (_heroes[i] == hero) {
					SelectHero(i);
					return;
				}
			}
		}
	}

	public void SelectHero(int heroIndex) {
		if (_heroes != null && heroIndex >= 0 && heroIndex < _heroes.Length) {
			_currentHeroIndex = heroIndex;
		}
	}

	public bool HaveHero(EUnitKey heroKey) {
		return GetHero(heroKey) != null;
	}

	public BaseHero GetHero(EUnitKey heroKey) {
		if (_heroes != null) {
			for (int i = 0; i < _heroes.Length; i++) {
				if (_heroes[i].Data.Key == heroKey) {
					return _heroes[i];
				}
			}
		}
		return null;
	}
}
