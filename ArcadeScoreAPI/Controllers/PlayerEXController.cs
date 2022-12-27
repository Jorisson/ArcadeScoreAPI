using ArcadeScoreAPI.Model;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ArcadeScoreAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayerEXController : ControllerBase
    {
        private string diretorio = "C:\\Users\\joris\\source\\repos\\ArcadeScoreAPI\\ArcadeScoreAPI\\scorearcade-43536-6044df62c1a1.json";
        private string projetoId;
        private FirestoreDb _firestoreDb;

        private readonly ILogger<PlayerEXController> _logger;

        public PlayerEXController(ILogger<PlayerEXController> logger)
        {
            _logger = logger;
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", diretorio);
            projetoId = "scorearcade-43536";
            _firestoreDb = FirestoreDb.Create(projetoId);
        }

        [HttpGet(Name = "GetPlayerEX")]
        public async Task<Player> GetAsync(string name)
        {
            name = name.ToUpper();
            Player player = new Player();
            Query pontuacaoQueryOrderBy = _firestoreDb.Collection("Pontuacao").WhereEqualTo("st_nome", name);
            QuerySnapshot pontuacaoQuerySnapshots = await pontuacaoQueryOrderBy.GetSnapshotAsync();

            List<Pontuacao> listarRanking = new List<Pontuacao>();
            int nr_recorde = 0;
            int nr_point_pivo = 0;
            string ult_data = "";

            foreach (DocumentSnapshot snapshot in pontuacaoQuerySnapshots.Documents)
            {
                if (snapshot.Exists)
                {
                    Dictionary<string, object> pontuacao = snapshot.ToDictionary();
                    string json = JsonConvert.SerializeObject(pontuacao);
                    Pontuacao newPoint = JsonConvert.DeserializeObject<Pontuacao>(json);
                    newPoint.Id = snapshot.Id;
                    listarRanking.Add(newPoint);

                    if (newPoint.nr_pontuacao > nr_point_pivo) {
                        nr_recorde++;
                        nr_point_pivo = newPoint.nr_pontuacao;
                    };
                    ult_data = newPoint.dt_Partida;
                }
            }
            if(listarRanking.Count > 0)
            {
                player.nr_jogadas = listarRanking.Count;
                player.nr_media_pont = Convert.ToInt32(listarRanking.Average(item => item.nr_pontuacao));
                player.nr_maior_pont = Convert.ToInt32(listarRanking.Max(item => item.nr_pontuacao));
                player.nr_menor_pont = Convert.ToInt32(listarRanking.Min(item => item.nr_pontuacao));
                player.nr_qtd_recorde = nr_recorde;

                TimeSpan ts = DateTime.Now.Subtract(Convert.ToDateTime(ult_data));
                DateTime periodo = new DateTime(ts.Ticks);

                player.dias_jogados = String.Concat(
                    periodo.Day >= 1 ? String.Format("{0} Dia(s)", periodo.Day): "",
                    periodo.Month - 1 == 1? String.Format(" e {0} Mês ", periodo.Month - 1): periodo.Month - 1 > 1 ? String.Format(" e {0} Meses ", periodo.Month - 1): "",
                    periodo.Year - 1 == 1? String.Format(" e {0} Ano ", periodo.Year - 1): periodo.Year - 1 > 1 ? String.Format(" e {0} Anos ", periodo.Year - 1) : ""
                );
            }

            return player;
        }
    }
}
