using System;

[Serializable]
public class StageData
{
	public string StageName;

	public int BestScore;

	public float BestTime;

	public int BestRings;

	public int BestTotalScore;

	public int BestRank;

	public StageData(string _StageName, int _BestScore, float _BestTime, int _BestRings, int _BestTotalScore, int _BestRank)
	{
		StageName = _StageName;
		BestScore = _BestScore;
		BestTime = _BestTime;
		BestRings = _BestRings;
		BestTotalScore = _BestTotalScore;
		BestRank = _BestRank;
	}
}
