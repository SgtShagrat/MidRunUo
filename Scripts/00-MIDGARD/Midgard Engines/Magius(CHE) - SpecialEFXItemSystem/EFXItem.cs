using System;
using Server;

namespace Midgard.Engines.SpecialEFXSystem
{
	public enum CycleModes
	{
		None,
		Linear,
		Bounce,
	}
	public delegate void OnEFXItemSequenceCompletedHandler(EFXItem item);
	public class EFXItem : Item
	{
		public DateTime EndAt{get;private set;}
		public DateTime StartAt{get;private set;}
		public object Tag;
		private InternalTimer m_Timer;
		private int ActualSequenceIndex = 0;
		private int[] Sequence;
		public CycleModes CycleMode{get;private set;}
		public bool Animated{get;private set;}
		private int AdvanceAccel = 1;
		private bool DuringUpdate=false;
		public TimeSpan SequenceDuration;
		public TimeSpan SequenceSingleFrameDuration;
		public event OnEFXItemSequenceCompletedHandler OnSequenceCompleted;
		public EFXItem ( Point3D loc, Map map, TimeSpan duration, TimeSpan delayeachframe, CycleModes cyclemode, params int[] sequenceofItemId )
				: base( sequenceofItemId[0])
		{
			Sequence = sequenceofItemId;
			CycleMode = cyclemode;
			Movable = false;
			MoveToWorld( loc, map );
			EndAt = DateTime.Now + duration;
			StartAt = DateTime.Now;
			SequenceDuration = duration;
			SequenceSingleFrameDuration = delayeachframe;
			m_Timer = new InternalTimer( this, delayeachframe );
			m_Timer.Start();
			Animated=false;
		}
		
		public int SequenceLength
		{
			get
			{
				if (Sequence!=null)
					return Sequence.Length;
				return 0;
			}
		}
		
		public int SequenceId(int index)
		{
			if (Sequence==null)
				return 0;
			if (index >= Sequence.Length)
				return 0;
			return Sequence[index];
		}
		
		public void EfxSequenceUpdate(TimeSpan duration, TimeSpan delayeachframe, params int[] sequenceofItemId)
		{
			DuringUpdate = true;
			m_Timer.Stop();
			m_Timer.Invalidated=true;
			EndAt = DateTime.Now + duration;
			StartAt = DateTime.Now;
			m_Timer = new InternalTimer( this, delayeachframe );
			Sequence = sequenceofItemId;
			ActualSequenceIndex = 0;
			if ( !CheckActualSequenceIndex() ) //realligned
				InvalidateItemGraphics();
			DuringUpdate = false;
			m_Timer.Start();
		}
		
		private bool CheckActualSequenceIndex()
		{
			if ((ActualSequenceIndex>=Sequence.Length || ActualSequenceIndex<=-1) && CycleMode == CycleModes.Bounce)
			{
				AdvanceAccel = -AdvanceAccel;
				ActualSequenceIndex+=AdvanceAccel;
				return false; //realligned
			}
			else if (ActualSequenceIndex>=Sequence.Length && CycleMode == CycleModes.Linear)
			{
				ActualSequenceIndex=0;
				return false; //realligned
			}
			//if (SpecialEFX.Debug)
			//	SpecialEFX.Pkg.LogInfoLine("EXFItem {0} CheckActualSequenceIndex {1} => {2}.",this.Name,ActualSequenceIndex,Sequence[ActualSequenceIndex]);
			return true;
		}
		
		public void UpdateAnimation()
		{
			if (CycleMode == CycleModes.None)
				return;
			
			var ellapsed = DateTime.Now.Subtract( StartAt ).TotalSeconds;
			var single = SequenceSingleFrameDuration.TotalSeconds;
			
			var actual = (int) ( ellapsed / single );
			if (actual>Sequence.Length)
			{
				var pair = ((int)(actual / Sequence.Length) % 2) == 0;
				actual = actual % Sequence.Length;
				if (pair && (CycleMode == CycleModes.Bounce))
				{
					actual = Sequence.Length-1-actual;
				}
			}
			ActualSequenceIndex = actual;
			CheckActualSequenceIndex();
			InvalidateItemGraphics();
		}
		private void InvalidateItemGraphics()
		{
			if (ActualSequenceIndex<0 || ActualSequenceIndex>=Sequence.Length)
			{
				SpecialEFX.Pkg.LogWarningLine("EXFItem {0} has invalid sequenceidx:{1}/{2}.",this.Name,ActualSequenceIndex,Sequence.Length);
				return;
			}
			
			var newid = Sequence[ActualSequenceIndex];
			if (newid==ItemID)
				return;
			ItemID=newid;			
		}
		public void Animate(bool enable)
		{
			if (Animated == enable)
				return;
			Animated = enable;
		}
		public override void OnAfterDelete()
		{
			base.OnAfterDelete();

			if ( m_Timer != null )
			{
				m_Timer.Invalidated=true;
				m_Timer.Stop();
			}
		}
		
		#region serialization
		public EFXItem( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.WriteDeltaTime( EndAt );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
				    Sequence = new int[0];
				    ActualSequenceIndex = 0;
					EndAt = reader.ReadDeltaTime();

					m_Timer = new InternalTimer( this, TimeSpan.Zero );
					m_Timer.Start();

					break;
				}
			}
		}
		#endregion
		
		private class InternalTimer : Timer
		{
			private EFXItem m_Item;
			public bool Invalidated=false;

			public InternalTimer( EFXItem item, TimeSpan delayeachframe ) : base( delayeachframe , delayeachframe )
			{
				m_Item = item;
				Invalidated=false;
			}

			protected override void OnTick()
			{
				if ( !Running )
					return;
				if ( m_Item.Deleted )
				{
					Stop();
					return;
				}

				if ( DateTime.Now > m_Item.EndAt )
				{
					if (!m_Item.DuringUpdate && m_Item.OnSequenceCompleted!=null)
					{
						m_Item.OnSequenceCompleted(m_Item);
					}
					if (!m_Item.DuringUpdate && !Invalidated)
						m_Item.Delete();										
					Stop();
				}
				if (m_Item.Animated)
				{
					m_Item.UpdateAnimation();
				}
			}
		}
	}
}

