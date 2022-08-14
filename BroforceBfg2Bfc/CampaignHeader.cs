using System;
using System.Xml.Serialization;

namespace BroforceBfg2Bfc
{
	[Serializable]
	public class CampaignHeader
	{
		// Token: 0x06002D33 RID: 11571 RVA: 0x00162C8D File Offset: 0x0016108D
		public void PrintEverything()
		{
		}

		// Token: 0x04002A03 RID: 10755
		public string name;

		// Token: 0x04002A04 RID: 10756
		public string author;

		// Token: 0x04002A05 RID: 10757
		public string description;

		// Token: 0x04002A06 RID: 10758
		public int length;

		// Token: 0x04002A07 RID: 10759
		public string md5;

		// Token: 0x04002A08 RID: 10760
		public bool hasBrotalityScoreboard;

		// Token: 0x04002A09 RID: 10761
		public bool hasTimeScoreBoard = true;

		// Token: 0x04002A0A RID: 10762
		[XmlIgnore]
		[NonSerialized]
		public bool isPublished;

		// Token: 0x04002A0B RID: 10763
		public GameMode gameMode;
	}

	public enum GameMode
	{
		// Token: 0x04003E23 RID: 15907
		Campaign,
		// Token: 0x04003E24 RID: 15908
		ExplosionRun,
		// Token: 0x04003E25 RID: 15909
		DeathMatch,
		// Token: 0x04003E26 RID: 15910
		BroDown,
		// Token: 0x04003E27 RID: 15911
		SuicideHorde,
		// Token: 0x04003E28 RID: 15912
		NotSet,
		// Token: 0x04003E29 RID: 15913
		Cutscene,
		// Token: 0x04003E2A RID: 15914
		Race
	}

}
// Token: 0x020003FC RID: 1020
