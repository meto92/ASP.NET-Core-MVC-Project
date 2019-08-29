using Microsoft.ML.Data;

namespace Metomarket.Web.MLModels
{
    public class ModelInput
    {
        [LoadColumn(0)]
        public string Description { get; set; }

        [LoadColumn(1)]
        public float Price { get; set; }
    }
}