using System.Threading.Tasks;

using Metomarket.Common;
using Metomarket.Web.MLModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.ML;

namespace Metomarket.Web.Hubs
{
    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    public class ProductCreateHub : Hub
    {
        private const string SetPriceMethodName = "SetPrice";

        private readonly PredictionEnginePool<ModelInput, ModelOutput> predictionEnginePool;

        public ProductCreateHub(PredictionEnginePool<ModelInput, ModelOutput> predictionEnginePool)
        {
            this.predictionEnginePool = predictionEnginePool;
        }

        public async Task GetPrice(string description)
        {
            ModelInput input = new ModelInput
            {
                Description = description,
            };

            ModelOutput output = this.predictionEnginePool.Predict(input);

            await this.Clients.All.SendAsync(SetPriceMethodName, output.Price);
        }
    }
}