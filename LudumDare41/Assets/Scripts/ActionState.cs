using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
	public class BehaviorGraph<T>
	{
		public interface IAction
		{
			IState Apply(IState state);
		}

		public interface IState
		{
			HashSet<IAction> GetPossibleNextActions();
		}
	}
}
