using System;
using System.Collections.Generic;
using System.Reflection;
using Server;

namespace Midgard.Engines.Packager
{
	public static class SecureConsole
	{
		public static void Write(string format,params object[] args)
		{
			if(args.Length==0)
				Console.Write(format);
			else
				Console.Write(string.Format(format,args));
		}
		public static void WriteLine()
		{
			Console.WriteLine();
		}
		public static void WriteLine(string format,params object[] args)
		{
			if(args.Length==0)
				Console.WriteLine(format);
			else
				Console.WriteLine(string.Format(format,args));
		}
		public static int BufferWidth
		{
			get
			{
				try			
				{
					return Console.BufferWidth;					
				}
				catch
				{
					return 80; //force width to 80
				}
			}
		}
		public static int BufferHeight
		{
			get
			{
				try			
				{
					return Console.BufferHeight;					
				}
				catch
				{
					return 40; //force width to 40
				}
			}
		}
		public static int CursorTop
		{
			get
			{
				try			
				{
					return Console.CursorTop;					
				}
				catch
				{
					return 0;
				}
			}
			set
			{
				try			
				{
					Console.CursorTop = value;
				}
				catch
				{
				}
			}			
		}
		public static int CursorLeft
		{
			get
			{
				try			
				{
					return Console.CursorLeft;					
				}
				catch
				{
					return 0;
				}
			}
			set
			{
				try			
				{
					Console.CursorLeft = value;
				}
				catch
				{
				}
			}
		}
	}
}

