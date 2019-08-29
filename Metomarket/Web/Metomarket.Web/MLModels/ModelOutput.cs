using Microsoft.ML.Data;

namespace Metomarket.Web.MLModels
{
    public class ModelOutput
    {
        private const string MLScoreColumnName = "Score";

        [ColumnName(MLScoreColumnName)]
        public float Price { get; set; }
    }
}