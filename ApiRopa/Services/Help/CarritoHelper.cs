namespace ApiRopa.Services.Help
{
    public static class CarritoHelper
    {
        private const int ID_INVITADO = 3;

        public static bool EsUsuarioInvitado(int usuarioId) => usuarioId == ID_INVITADO;

        public static async Task<bool> ReservarStockSiNoEsInvitadoAsync(IProductoTallaRepositorio repo, int productoTallaId, int cantidad, int usuarioId, ILogger logger)
        {
            if (EsUsuarioInvitado(usuarioId))
            {
                logger.LogInformation("ℹ️ No se reserva stock para usuario invitado (ID: {UsuarioId})", usuarioId);
                return true;
            }

            return await repo.ReservarStockAsync(productoTallaId, cantidad);
        }

        public static async Task LiberarStockSiNoEsInvitadoAsync(IProductoTallaRepositorio repo, int productoTallaId, int cantidad, int usuarioId, ILogger logger)
        {
            if (!EsUsuarioInvitado(usuarioId))
                await repo.LiberarStockAsync(productoTallaId, cantidad);
        }
    }
}
