using Microsoft.AspNetCore.Mvc;
using WebApi.Domain.Enum;
using WebApi.Domain.Model;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoriesDtoController : ControllerBase
    {      

        private readonly ILogger<CategoriesDtoController> _logger;

        public CategoriesDtoController(ILogger<CategoriesDtoController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetCategories")]
        public CategoriesDto Get()
        {
            return new CategoriesDto
            {
                Categorias = new List<CategoryDetailDto>
                {
                    new CategoryDetailDto
                    {
                        Categoria = ECategory.Iluminacion,
                        Items = new List<ItemDto>
                        {
                            new ItemDto { Elemento = "Lámpara Led de 5w", Valor = 5 },
                            new ItemDto { Elemento = "Lámpara Led de 10w", Valor = 10 },
                            new ItemDto { Elemento = "Lámpara Incandescente 40w", Valor = 40 },
                            new ItemDto { Elemento = "Lámpara Incandescente de 100w", Valor = 100 },
                            new ItemDto { Elemento = "Lámpara Incandescente de 200w", Valor = 200 }
                        }
                    },
                    new CategoryDetailDto
                    {
                        Categoria = ECategory.Refrigeracion,
                        Items = new List<ItemDto>
                        {
                            new ItemDto { Elemento = "Heladera", Valor = 1000 },
                            new ItemDto { Elemento = "Freezer", Valor = 1500 }
                        }
                    }
                }
            };
        }
    }
}
