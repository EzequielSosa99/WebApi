using WebApi.Domain.Enum;

namespace WebApi.Domain.Model
{
    public class CategoryDetailDto
    {
        public ECategory Categoria { get; set; }
        public IEnumerable<ItemDto> Items { get; set; }
    }
}
