namespace MapeamentoUrbano.Domain.Constants
{
    public class OperationMessage
    {
        public const string STATUS_200_OK = "Operação com sucesso.";
        public const string STATUS_201_CREATED = "Criado com sucesso.";
        public const string STATUS_400_INVALID_PAYLOAD = "Campos requeridos não foram preenchidos.";
        public const string STATUS_400_BAD_REQUEST = "Informações conflitantes.";
        public const string STATUS_400_CONTAIN_CHILDREN = "Não é possível deletar categorias com itens.";
        public const string STATUS_400_INVALID_PAYLOAD_INCONSISTENT = "Campos preenchidos estão inconsistentes.";
        public const string STATUS_400_UNABLE_TO_INACTIVATE = "Não foi possível inativar a pesquisa anterior.";
        public const string STATUS_404_NOT_FOUND = "Elemento não encontrado";
        public const string STATUS_422_FAIL_OPERATION_DATABASE = "Falha ao executar a operação no banco de dados.";
        public const string STATUS_500_INTERNAL_SERVER_ERROR = "Falha na operação.";
    }
}
