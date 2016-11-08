using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using RimWorld;

namespace CleanNearbyRockRubble
{
	internal class MarvsWorkGiver_OverrideClean : WorkGiver_Scanner
	{
		// Only dirt that has been around for a long time will be cleaned by this WorkGiver.
		private int MinTicksSinceThickened = 2400;

		public override PathEndMode PathEndMode
		{
			get
			{
				return PathEndMode.OnCell;
			}
		}

		public override ThingRequest PotentialWorkThingRequest
		{
			get
			{
				return ThingRequest.ForGroup(ThingRequestGroup.Filth);
			}
		}

		public override int LocalRegionsToScanFirst
		{
			get
			{
				return 4;
			}
		}

		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn Pawn)
		{
			return ListerFilthInHomeArea.FilthInHomeArea;
		}

		public override bool HasJobOnThing(Pawn pawn, Thing t)
		{
			if (pawn.Faction != Faction.OfPlayer)
			{
				return false;
			}

			Filth filth = t as Filth;

			if (filth == null)
			{
				return false;
			}

			int pMining = pawn.workSettings.GetPriority(WorkTypeDefOf.Mining);
			int pCleaning = pawn.workSettings.GetPriority(WorkTypeDefOf.Cleaning);
			// Remember that with priorities, higher numbers mean LOWER priority.

			if (filth.def.defName != "RockRubble")
			{
				// If Mining job is turned off, or mining job has greater or equal priority to cleaning
				if (pMining == 0 || pMining <= pCleaning)
				{
					return Find.AreaHome[filth.Position] && pawn.CanReserveAndReach(t, PathEndMode.ClosestTouch, pawn.NormalMaxDanger(), 1) && filth.TicksSinceThickened >= this.MinTicksSinceThickened;

				}
				// Otherwise, if 9600 ticks have passed, this filth can be cleaned.
				return Find.AreaHome[filth.Position] && pawn.CanReserveAndReach(t, PathEndMode.ClosestTouch, pawn.NormalMaxDanger(), 1) && filth.TicksSinceThickened >= 4 * this.MinTicksSinceThickened;
			}

			// We've done rock rubble with the higher-priority WorkGivers.
			return false;

		}

		public override Job JobOnThing(Pawn pawn, Thing t)
		{
			return new Job(JobDefOf.Clean, t);
		}
	}
}


