using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using RimWorld;

namespace CleanNearbyRockRubble
{
	internal class MarvsWorkGiver_CleanRockRubble : WorkGiver_Scanner
	{
		private int MinTicksSinceThickened = 300;

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
				return ThingRequest.ForDef(ThingDef.Named("RockRubble"));
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

			// If mining is not a job the pawn has, then don't prioritise cleaning nearby rubble
			if (pawn.workSettings.GetPriority(WorkTypeDefOf.Mining) == 0)
			{
				return false;
			}

			// If mining has a higher priority (lower value but not zero, already eliminated above)
			// than cleaning, then don't prioritise cleaning nearby rubble.
			if (pawn.workSettings.GetPriority(WorkTypeDefOf.Mining) <= pawn.workSettings.GetPriority(WorkTypeDefOf.Cleaning))
			{
				return false;
			}

			Filth filth = t as Filth;

			if (filth == null)
			{
				return false;
			}

			IntVec3 dist = pawn.Position - filth.Position;
			// If the distance between the filth position and the pawn's current position is more than 10 tiles
			// don't prioritise.
			if (dist.LengthManhattan > 10)
			{
				return false;
			}


			return Find.AreaHome[filth.Position] && pawn.CanReserveAndReach(t, PathEndMode.ClosestTouch, pawn.NormalMaxDanger(), 1) && filth.TicksSinceThickened >= this.MinTicksSinceThickened;
		}

		public override Job JobOnThing(Pawn pawn, Thing t)
		{
			return new Job(JobDefOf.Clean, t);
		}
	}
}


