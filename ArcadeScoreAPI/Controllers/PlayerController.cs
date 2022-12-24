using ArcadeScoreAPI.Model;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ArcadeScoreAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayerController : ControllerBase
    {
        private string diretorio = "C:\\Users\\joris\\source\\repos\\ArcadeScoreAPI\\ArcadeScoreAPI\\scorearcade-43536-6044df62c1a1.json";
        private string projetoId;
        private FirestoreDb _firestoreDb;

        private readonly ILogger<PlayerController> _logger;

        public PlayerController(ILogger<PlayerController> logger)
        {
            _logger = logger;
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", diretorio);
            projetoId = "scorearcade-43536";
            _firestoreDb = FirestoreDb.Create(projetoId);
        }

        [HttpGet(Name = "GetPlayer")]
        public async Task<IEnumerable<Pontuacao>> GetAsync()
        {
            Query pontuacaoQueryOrderBy = _firestoreDb.Collection("Pontuacao").OrderByDescending("nr_pontuacao").Limit(10);
            QuerySnapshot pontuacaoQuerySnapshots = await pontuacaoQueryOrderBy.GetSnapshotAsync();
            List<Pontuacao> listarRanking = new List<Pontuacao>();

            foreach (DocumentSnapshot snapshot in pontuacaoQuerySnapshots.Documents)
            {
                if (snapshot.Exists)
                {
                    Dictionary<string, object> pontuacao = snapshot.ToDictionary();
                    string json = JsonConvert.SerializeObject(pontuacao);
                    Pontuacao newPoint = JsonConvert.DeserializeObject<Pontuacao>(json);
                    newPoint.Id = snapshot.Id;
                    listarRanking.Add(newPoint);
                }
            }

            return listarRanking;
        }

        [HttpPost(Name = "PostPlayer")]
        public async Task<string> NovaPontuacao( Pontuacao pontuacao) {
            CollectionReference crPontuacao = _firestoreDb.Collection("Pontuacao");
            await crPontuacao.AddAsync(pontuacao);
            return "Sucesso!";
        }
    }
}