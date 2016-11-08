using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using RimWorld;

namespace CleanNearbyRockRubble
{
	internal class MarvsWorkGiver_ContingencyClean : WorkGiver_Scanner
	{
		private int MinTicksSinceThickened = 600;

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
				// Otherwise, miners cleaning the mine don't clean this using this JobGiver.
				return false;
			}

			// This must be rubble.
			// If mining is enabled and it is lower priority than cleaning
			if (pMining != 0 && pMining > pCleaning)
			{
				IntVec3 dist = pawn.Position - filth.Position;
				// If the distance between the filth position and the pawn's current position is more than 25 tiles
				// don't prioritise.
				if (dist.LengthManhattan > 25)
				{
					return false;
				}
				// This will become a job if THREE TIMES the usual number of ticks have passed.
				return Find.AreaHome[filth.Position] && pawn.CanReserveAndReach(t, PathEndMode.ClosestTouch, pawn.NormalMaxDanger(), 1) && filth.TicksSinceThickened >= 3 * this.MinTicksSinceThickened;
			}
			// Cleaners can still clean rubble but will prefer to leave it to the mine cleaners unless
			// SIX TIMES the usual number of ticks have passed.
			return Find.AreaHome[filth.Position] && pawn.CanReserveAndReach(t, PathEndMode.ClosestTouch, pawn.NormalMaxDanger(), 1) && filth.TicksSinceThickened >= 6 * this.MinTicksSinceThickened;

		}

		public override Job JobOnThing(Pawn pawn, Thing t)
		{
			return new Job(JobDefOf.Clean, t);
		}
	}
}


