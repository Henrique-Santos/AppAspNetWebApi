using Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Api.ViewModels
{
    // Binder personalizado para envio de IFormFile e ViewModel dentro de um FormData. Compatível com .NET Core 3.1 ou superior (system.text.json)
    [ModelBinder(BinderType = typeof(ProdutoModelBinder))]
    public class ProdutoImagemViewModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public Guid FornecedorId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O campo {0} precisa estar entre {2} e {1}")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(1000, MinimumLength = 2, ErrorMessage = "O campo {0} precisa estar entre {2} e {1}")]
        public string Descricao { get; set; }

        // IFormFile implementa stream de dados
        public IFormFile ImagemUpload { get; set; }

        public string NomeImagem { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public decimal Valor { get; set; }

        [ScaffoldColumn(false)]
        public string NomeFornecedor { get; set; }

        [ScaffoldColumn(false)]
        public DateTime DataCadastro { get; set; }

        public bool Ativo { get; set; }                
    }
}