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

        private async Task<List<InventoryItem>> RequestItems(string characterName) => await dbContext.InventoryItems
                .Where(item => item.CharacterInventoryCharacterN == characterName)
                .ToListAsync();

        private async Task<List<QuestRequirement>> RequestRequirements(int questId) => await dbContext.QuestRequirements
                .Where(req => req.QuestId == questId)
                .Include(req => req.Item)
                .ToListAsync();

        private Dictionary<int, int> RequirementsToDict(IEnumerable<QuestRequirement> requirements)
        {
            var dict = new Dictionary<int, int>();
            foreach (var req in requirements)
            {
                dict[req.ItemId] = req.Amount;
            }
            return dict;
        }

        private bool ConsumeStacks(IEnumerable<InventoryItem> items, IDictionary<int, int> requirements, bool canConsume)
        {
            foreach (var item in items)
            {
                if (!requirements.ContainsKey(item.ItemId))
                    continue;

                var sub = Math.Min(requirements[item.ItemId], item.StackCount);
                requirements[item.ItemId] -= sub;

                if (requirements[item.ItemId] <= 0)
                    requirements.Remove(item.ItemId);

                if (canConsume)
                {
                    item.StackCount -= sub;
                    if (item.StackCount == 0)
                    {
                        dbContext.InventoryItems.Remove(item);
                    }
                    else if (item.StackCount < 0)
                        throw new Exception("Consuming stack made a negative stack count");
                }

                if (requirements.Count == 0)
                    return true;
            }
            return false;
        }

        [HttpGet("{npcId}/offered-quests")]
        public async Task<ObjectResult> GetOfferedQuests(int npcId, string characterName)
        {
            if (!await auth.ValidateAsync(Request.Headers, characterName))
                return StatusCode(403, "Unauthorized");

            var character = await dbContext.Characters.SingleAsync(character => character.CharacterName == characterName);
            var quests = await dbContext.Quests
                .Where(quest => quest.NpcId == npcId
                    && quest.IsOffered
                    && quest.Level <= character.Level
                )
                .Include(quest => quest.AcceptedQuests.Where(accQuest => accQuest.CharacterName == characterName))
                .Include(quest => quest.QuestReward)
                .Include(quest => quest.QuestRequirements)
                    .ThenInclude(requirement => requirement.Item)
                .ToListAsync();

            return Ok(quests);
        }

        [HttpGet("{characterName}/quest-completable")]
        public async Task<ObjectResult> GetQuestCompleteable(string characterName, int questId)
        {
            var items = await RequestItems(characterName);
            var required = await RequestRequirements(questId);
            return Ok(ConsumeStacks(items, RequirementsToDict(required), false));
        }

        [HttpPost("{characterName}/accept-quest")]
        public async Task<ObjectResult> AcceptQuest(string characterName, [FromBody] Dictionary<string, string> paramters)
        {
            var questId = int.Parse(paramters["questId"]);

            if (!await auth.ValidateAsync(Request.Headers, characterName))
                return StatusCode(403, "Unauthorized");

            var quest = await dbContext.Quests
                .Where(quest => quest.QuestId == questId)
                .Include(quest => quest.AcceptedQuests.Where(accQ => accQ.CharacterName == characterName))
                .SingleAsync();

            AcceptedQuest acceptedQuest;
            if (quest.AcceptedQuests.Any())
            {
                acceptedQuest = quest.AcceptedQuests.First();
                acceptedQuest.QuestState = (int)QuestState.Accepted;
            }
            else
            {
                acceptedQuest = new AcceptedQuest()
                {
                    CharacterName = characterName,
                    Quest = quest,
                    QuestState = (int)QuestState.Accepted,
                };
                dbContext.Add(acceptedQuest);
            }

            await dbContext.SaveChangesAsync();

            return Ok(acceptedQuest);
        }

        [HttpPost("{characterName}/abandon-quest")]
        public async Task<ObjectResult> AbandonQuest(string characterName, [FromBody] Dictionary<string, string> paramters)
        {
            var questId = int.Parse(paramters["questId"]);

            if (!await auth.ValidateAsync(Request.Headers, characterName))
                return StatusCode(403, "Unauthorized");

            var acceptedQuest = await dbContext.AcceptedQuests
                .SingleAsync(quest => quest.QuestId == questId && quest.CharacterName == characterName);
            acceptedQuest.QuestState = (int)QuestState.NotAccepted;

            await dbContext.SaveChangesAsync();

            return Ok(acceptedQuest);
        }

        [HttpPost("{characterName}/complete-quest")]
        public async Task<ObjectResult> CompleteQuest(string characterName, [FromBody] Dictionary<string, string> paramters)
        {
            var questId = int.Parse(paramters["questId"]);

            var items = await RequestItems(characterName);
            var required = await RequestRequirements(questId);

            if (ConsumeStacks(items, RequirementsToDict(required), true))
            {
                var acceptedQuest = await dbContext.AcceptedQuests
                    .SingleAsync(quest => quest.QuestId == questId && quest.CharacterName == characterName);
                acceptedQuest.QuestState = (int)QuestState.Completed;

                await dbContext.SaveChangesAsync();
                return Ok(items);
            }
            else
                return Ok(null);
        }
    }
}
