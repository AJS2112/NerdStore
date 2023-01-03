using NerdStore.Core.DomainObjects;

namespace NerdStore.Catalogo.Domain.Tests
{
    public class ProdutoTests
    {
        [Fact]
        public void Produto_Validar_ValidacoesDevemRetornarExceptions()
        {
            // Arrange & Act & Assert
            var ex = Assert.Throws<DomainException>(() =>
                new Produto(String.Empty, "Descricao", false, 100, Guid.NewGuid(), DateTime.Now, "Imagem", new Dimensoes(2,2,2))
            ) ;
            Assert.Equal("O campo Nome do produto não pode estar vazio", ex.Message);

            ex = Assert.Throws<DomainException>(() =>
                new Produto("Nome", String.Empty, false, 100, Guid.NewGuid(), DateTime.Now, "Imagem", new Dimensoes(2, 2, 2))
            );
            Assert.Equal("O campo Descricao do produto não pode estar vazio", ex.Message);

            ex = Assert.Throws<DomainException>(() =>
                new Produto("Nome", "Descricao", false, 100, Guid.Empty, DateTime.Now, "Imagem", new Dimensoes(2, 2, 2))
            );
            Assert.Equal("O campo Categoria do produto não pode estar vazio", ex.Message);

            ex = Assert.Throws<DomainException>(() =>
                new Produto("Nome", "Descricao", false, 0, Guid.NewGuid(), DateTime.Now, "Imagem", new Dimensoes(2, 2, 2))
            );
            Assert.Equal("O campo Valor do produto não pode estar vazio", ex.Message);

            ex = Assert.Throws<DomainException>(() =>
                new Produto("Nome", "Descricao", false, 100, Guid.NewGuid(), DateTime.Now, "", new Dimensoes(2, 2, 2))
            );
            Assert.Equal("O campo Imagem do produto não pode estar vazio", ex.Message);

        }
    }
}