using Metomarket.Data.Common.Models;

namespace Metomarket.Data.Models
{
    public class Setting : BaseDeletableModel<int>
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }
}