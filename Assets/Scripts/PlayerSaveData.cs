using System;
[System.Serializable]
public class PlayerSaveData {

	public int money;
	public int specialMoney;
	public int bestScore;
	public bool[] ownedCharacters;
	public int choosedCharacter;
	public DateTime freePrizeCollectedDate;
	public bool isMusicMuted;
	public bool isSoundMuted;
	public bool playedBefore;
	public int gamesPlayed;
	public bool developerMode;
	public bool isSharedPost;

	public PlayerSaveData (int money, int specialMoney, int bestScore, bool[] ownedCharacters, int choosedCharacter, DateTime freePrizeCollectedDate, bool isMusicMuted, bool isSoundMuted, bool playedBefore, int gamesPlayed, bool developerMode, bool isSharedPost)
	{
		this.money = money;
		this.specialMoney = specialMoney;
		this.bestScore = bestScore;
		this.ownedCharacters = ownedCharacters;
		this.choosedCharacter = choosedCharacter;
		this.freePrizeCollectedDate = freePrizeCollectedDate;
		this.isMusicMuted = isMusicMuted;
		this.isSoundMuted = isSoundMuted;
		this.playedBefore = playedBefore;
		this.gamesPlayed = gamesPlayed;
		this.developerMode = developerMode;
		this.isSharedPost = isSharedPost;
	}

}
