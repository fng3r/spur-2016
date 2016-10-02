using System.Linq;
using SpurRoguelike.Core;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;
using System.Collections.Generic;

namespace SpurRoguelike.PlayerBot
{
    public class PlayerBot : IPlayerController
    {
        private bool needHealth;
        private PawnView target;
        private HealthPackView pack;
        private ItemView coreItem;
        private Location exit;

        public Turn MakeTurn(LevelView levelView, IMessageReporter messageReporter)
        {
            messageReporter.ReportMessage($"Hey ho! I'm still breathing. {levelView.Player.Location}");
            

            exit = levelView.Field.GetCellsOfType(CellType.Exit).Single();
            coreItem = levelView.Items.OrderByDescending(x => GetItemValue(x)).FirstOrDefault();

            var player = levelView.Player;
            var freeCells = levelView.Field.GetCellsOfType(CellType.Empty);

            if (target.HasValue && target.Health > 0)
                GetReady(player);

            if (levelView.Monsters.Count() == 0)
            {
                if (player.Health != 100 && levelView.HealthPacks.Count() != 0)
                {
                    pack = levelView.HealthPacks.MinBy(x => x.Location - player.Location, new OffsetComparer());
                    return Step(player.Location, pack.Location, levelView);
                }

                ItemView myItem;
                if (player.TryGetEquippedItem(out myItem))
                {
                    if (GetItemValue(myItem) < GetItemValue(coreItem))
                        return Step(player.Location, coreItem.Location, levelView);
                }
                else
                    return Step(player.Location, coreItem.Location, levelView);

            return Step(player.Location, exit, levelView);
            }
            
            
            if (needHealth)
            {
                if (levelView.HealthPacks.Count() == 0)
                    return Step(player.Location, exit, levelView);
                if (!pack.HasValue || !levelView.HealthPacks.Contains(pack))
                    pack = levelView.HealthPacks.MinBy(x => x.Location - player.Location, new OffsetComparer());

                return Step(player.Location, pack.Location, levelView);
            }

            if (!needHealth)
            {
                ItemView myItem;
                if (player.TryGetEquippedItem(out myItem))
                {
                    if (GetItemValue(myItem) < GetItemValue(coreItem))
                        return Step(player.Location, coreItem.Location, levelView);
                }
                else
                    return Step(player.Location, coreItem.Location, levelView);
            }

            var nearbyMonsters = levelView.Monsters.Where(x => IsInAttackRange(player.Location, x.Location));
            var nearbyMonster = nearbyMonsters.FirstOrDefault();

            
            if (!target.HasValue || target.Health <= 0)
                target = GetNearestMonster(levelView.Monsters, player);

            if (nearbyMonster.HasValue && !needHealth)
            {
                if (nearbyMonsters.Count() < 2)
                {
                    target = nearbyMonster;
                    return Turn.Attack(nearbyMonster.Location - player.Location);
                }
                else
                    return Retreat(player, freeCells, levelView);
            }

            return Step(player.Location, target.Location, levelView);

        }

        private Turn Retreat(PawnView player, IEnumerable<Location> freeCells, LevelView levelView)
        {
            var retreatCells = Offset.StepOffsets.Select(x => player.Location + x).Where(x => freeCells.Contains(x));
            var choice = retreatCells
                .OrderBy(x => levelView.Monsters.Where(m => IsInAttackRange(m.Location, x)).Count())
                .First();

            return Step(player.Location, choice, levelView);
        }


        private Turn Step(Location from, Location to, LevelView levelView)
        {
            var offset = to - from;
            var snappedOffset = offset.SnapToStep();

            var freeCells = levelView.Field.GetCellsOfType(CellType.Empty)
                .Except(levelView.Monsters.Select(x => x.Location))
                .ToHashSet();

            freeCells.Add(to);
            freeCells.Add(exit);
                
            var path = PathFinder.BFS(from, to, freeCells);
            if (path == null)
                snappedOffset = offset.AndvancedSnapToStep();
            else
                snappedOffset = Offset.StepOffsets.FirstOrDefault(x => from + x == path[1]);

            return Turn.Step(snappedOffset);
        }

        private void GetReady(PawnView player) => ReadyToFight(player);

        private void ReadyToFight(PawnView player)
        {
            if (player.Health < 70)
                needHealth = true;
            else
                needHealth = false;
        }

        private bool TryGetCoreItem(PawnView player, LevelView levelView, ref Turn turn)
        {
            var coreItem = levelView.Items.OrderByDescending(x => GetItemValue(x)).First();
            ItemView item;
            player.TryGetEquippedItem(out item);
            if (item.HasValue && GetItemValue(item) < GetItemValue(coreItem))
            {
                turn = Step(player.Location, coreItem.Location, levelView);
                return true;
            }
            return false;

        }

        private static double GetItemValue(ItemView item) => item.AttackBonus * 1.2 + item.DefenceBonus * 1.5;

        private static bool IsInAttackRange(Location a, Location b) => a.IsInRange(b, 1);

        private PawnView GetNearestMonster(IEnumerable<PawnView> entities, PawnView player) => entities
            .MinBy(x => player.Location - x.Location, new OffsetComparer());
    }
    
}
