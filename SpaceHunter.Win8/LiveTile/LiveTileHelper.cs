using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;

namespace ScribbleHunter.Windows.LiveTile
{
    public static class LiveTileHelper
    {
        private const int LIFETIME = 3;

        /// <summary>
        /// Shows the Highscore livetile.
        /// </summary>
        /// <param name="bestRank">The best rank of the player</param>
        /// <param name="bestScore">The best score of the player</param>
        public static void ShowHighscoreLiveTile(int bestRank, long bestScore)
        {
            var tile = GetTile("Best Rank", bestRank.ToString(), "Best Score", bestScore.ToString());
            var tileUpdater = TileUpdateManager.CreateTileUpdaterForApplication();
            tileUpdater.Update(tile);
        }

        /// <summary>
        /// Gets the livetile notification object with 4 text lines.
        /// </summary>
        /// <param name="line1">The first line</param>
        /// <param name="line2">The second line</param>
        /// <param name="line3">The third line</param>
        /// <param name="line4">The fourth line</param>
        /// <returns></returns>
        private static TileNotification GetTile(string line1, string line2, string line3, string line4)
        {
            var templateType = TileTemplateType.TileSquareText03;
            var xml = TileUpdateManager.GetTemplateContent(templateType);
            var textNodes = xml.GetElementsByTagName("text");
            textNodes[0].AppendChild(xml.CreateTextNode(line1));
            textNodes[1].AppendChild(xml.CreateTextNode(line2));
            textNodes[2].AppendChild(xml.CreateTextNode(line3));
            textNodes[3].AppendChild(xml.CreateTextNode(line4));

            var tile = new TileNotification(xml);
            tile.ExpirationTime = new DateTimeOffset(DateTime.Now.AddDays(LIFETIME));
            return tile;
        }
    }
}
