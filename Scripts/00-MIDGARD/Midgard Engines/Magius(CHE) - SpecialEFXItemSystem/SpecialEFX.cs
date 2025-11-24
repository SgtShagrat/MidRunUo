using System;
using Server;
using Midgard.Engines.Packager;
namespace Midgard.Engines.SpecialEFXSystem
{
	public class SpecialEFX
	{
		public static object[] Package_Info =
        {
            "Script Title", "Special EFX Items Engine",
            "Enabled by Default", true,
            "Script Version", new Version( 1, 0, 0, 0 ),
            "Author name", "Magius(CHE)",
            "Creation Date", new DateTime( 2011, 5, 7 ),
            "Author mail-contact", "cheghe@tiscali.it",
            "Author home site", "http://www.magius.it",
            //"Author notes",           null,
            "Script Copyrights", "(C) Midgard Shard - Magius(CHE",
            "Provided packages", new string[] { "Midgard.Engines.SpecialEFXSystem" },
            /*"Required packages",       new string[]{"Midgard.Engines.SkillSystem"},*/
            //"Conflicts with packages",new string[0],
            "Research tags", new string[] { "SpecialEFXSystem", "Effect"}
        };
		
#if DEBUG
        public static bool Debug = true;
#else
		public static bool Debug = false;
#endif
		
		private SpecialEFX ()
		{
		}
		
		internal static Package Pkg;
		
        public static bool Enabled { get { return Pkg.Enabled; } set { Pkg.Enabled = value; } }

        public static void Package_Configure()
        {
            Pkg = Packager.Core.Singleton[ typeof( SpecialEFX ) ];			
        }

        public static void Package_Initialize()
        {
		}
		//0x1fdf,  p, Caster.Map,  TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(3.0/11.0), 11
		public static EFXItem CreateEffect(IPoint3D dest, Map map, int initialid, int consecutiveframescount, TimeSpan totalDelay)
		{
			var p = ( dest is Point3D ) ? (Point3D)dest : new Point3D( dest.X,dest.Y,dest.Z );
			return CreateEffect(new Entity(Serial.Zero, p, map),initialid,consecutiveframescount,totalDelay);
		}
		public static EFXItem CreateEffect(IEntity target, int initialid, int consecutiveframescount, TimeSpan totalDelay)
		{
			var sequence = new int[Math.Abs(consecutiveframescount)];
			for(var h=0;h<sequence.Length;h++)
				sequence[h]=initialid + ( consecutiveframescount>0 ? h : -h);
			var item = new EFXItem(target.Location,target.Map, totalDelay, TimeSpan.FromSeconds ( totalDelay.TotalSeconds / consecutiveframescount ), CycleModes.Linear,sequence);
			item.Animate(true);
			return item;
		}
		
		public static void ReverseEfx(EFXItem efx)
		{
			UpdateEfx( efx, efx.Location, efx.Map, efx.SequenceId(efx.SequenceLength-1), -efx.SequenceLength , efx.SequenceDuration );		
		}
		
		public static void UpdateEfx(EFXItem efx, IPoint3D dest, Map map, int initialid, int consecutiveframescount, TimeSpan totalDelay)
		{
			var p = ( dest is Point3D ) ? (Point3D)dest : new Point3D( dest.X,dest.Y,dest.Z );
			UpdateEfx(efx,new Entity(Serial.Zero,p,map),initialid,consecutiveframescount,totalDelay);
		}
		public static void UpdateEfx(EFXItem efx, IEntity target, int initialid, int consecutiveframescount, TimeSpan totalDelay)
		{
			var sequence = new int[Math.Abs(consecutiveframescount)];
			for(var h=0;h<sequence.Length;h++)
				sequence[h]=initialid + ( consecutiveframescount>0 ? h : -h);
			var newsequencedelay = TimeSpan.FromSeconds ( totalDelay.TotalSeconds / consecutiveframescount );
			efx.Animate(false);
			efx.EfxSequenceUpdate(totalDelay,newsequencedelay,sequence);
			efx.MoveToWorld(target.Location,target.Map);
			efx.Animate(true);
		}
		//public static EFXItem 
	}
}

