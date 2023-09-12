using System.ComponentModel.DataAnnotations;

namespace Api.ViewModels
{
    public class FornecedorViewModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O campo {0} precisa estar entre {2} e {1}")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(14, MinimumLength = 11, ErrorMessage = "O campo {0} precisa estar entre {2} e {1}")]
        public string Documento { get; set; }

        public int TipoFornecedor { get; set; }

        public bool Ativo { get; set; }        

        public EnderecoViewModel Endereco { get; set; }

        public IEnumerable<ProdutoViewModel> Produtos { get; set; }
    }
}