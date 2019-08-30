using Metomarket.Web.MLModels;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ML;

namespace Metomarket.Web.Api
{
    public class SuggestPriceController : ApiController
    {
        private const string EmptyString = "";

        private readonly PredictionEnginePool<ModelInput, ModelOutput> predictionEnginePool;

        public SuggestPriceController(PredictionEnginePool<ModelInput, ModelOutput> predictionEnginePool)
        {
            this.predictionEnginePool = predictionEnginePool;
        }

        [HttpGet(EmptyString)]
        public ActionResult<float> SuggestPrice(string name)
        {
            ModelInput input = new ModelInput
            {
                Description = name,
            };

            ModelOutput output = this.predictionEnginePool.Predict(input);

            return output.Price;
        }
    }
}