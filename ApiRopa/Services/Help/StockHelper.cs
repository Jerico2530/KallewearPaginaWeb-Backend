namespace ApiRopa.Services.Help
{
    public class StockHelper
    {
        private const int ID_INVITADO = 3; // ID de invitado fijo, ajusta según tu proyecto

        public static bool EsUsuarioInvitado(int usuarioId) => usuarioId == ID_INVITADO;

        // Reservar stock si el usuario NO es invitado
        public static async Task<bool> ReservarStockSiNoEsInvitadoAsync(
            IProductoTallaRepositorio productoTallaRepo,
            int productoTallaId,
            int cantidad,
            int usuarioId,
            ILogger logger)
        {
            if (EsUsuarioInvitado(usuarioId))
            {
                logger.LogInformation("ℹ️ No se reserva stock para usuario invitado (ID: {UsuarioId})", usuarioId);
                return true;
            }

            return await productoTallaRepo.ReservarStockAsync(productoTallaId, cantidad);
        }

        // Liberar stock si el usuario NO es invitado
        public static async Task LiberarStockSiNoEsInvitadoAsync(
            IProductoTallaRepositorio productoTallaRepo,
            int productoTallaId,
            int cantidad,
            int usuarioId,
            ILogger logger)
        {
            if (!EsUsuarioInvitado(usuarioId))
                await productoTallaRepo.LiberarStockAsync(productoTallaId, cantidad);
        }
    }
}

