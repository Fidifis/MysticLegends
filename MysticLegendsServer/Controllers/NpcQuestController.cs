using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MysticLegendsServer.Models;
using MysticLegendsShared.Models;
using MysticLegendsShared.Utilities;

namespace MysticLegendsServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NpcQuestController : Controller
    {
        private Xdigf001Context dbContext;
        private ILogger<CharacterController> logger;
        private Auth auth;

        public NpcQuestController(Xdigf001Context context, ILogger<CharacterController> logger, Auth auth)
        {
            dbContext = context;
            this.logger = logger;
            this.auth = auth;
        }

        [HttpGet("{npcId}/offered-quests")]
        public async Task<ObjectResult> GetOfferedQuests(int npcId, string characterName)
        {
            if (!await auth.ValidateAsync(Request.Headers, characterName))
                return StatusCode(403, "Unauthorized");

            var quests = dbContext.Quests
                .Where(quest => quest.NpcId == npcId && quest.IsOffered)
                //.Include(quest => quest.AcceptedQuests)
                .Include(quest => quest.AcceptedQuests.Where(accQuest => accQuest.CharacterName == characterName))
                .ToListAsync();

            return Ok(quests);
        }

        [HttpPost("{characterName}/accept-quest")]
        public async Task<ObjectResult> AcceptQuestPrice(string characterName, [FromBody] Dictionary<string, string> paramters)
        {
            var questId = int.Parse(paramters["questId"]);

            if (!await auth.ValidateAsync(Request.Headers, characterName))
                return StatusCode(403, "Unauthorized");

            var quest = await dbContext.Quests.SingleAsync(quest => quest.QuestId == questId);

            var acceptedQuest = new AcceptedQuest()
            {
                CharacterName = characterName,
                Quest = quest,
                QuestState = (int)QuestState.Accepted,
            };

            dbContext.Add(acceptedQuest);
            await dbContext.SaveChangesAsync();

            return Ok(acceptedQuest);
        }
    }
}
