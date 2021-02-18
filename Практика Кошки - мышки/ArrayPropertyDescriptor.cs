// Static Model

using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Практика_Кошки___мышки
{
    /// <summary>
	/// Property decriptor for array
	/// </summary>
	public class ArrayPropertyDescriptor: PropertyDescriptor
	{
        private int _index;

		public ArrayPropertyDescriptor(string name,Type type,int index) : base (name,null)
		{
			DisplayName	= name;
			PropertyType	= type;
			_index  = index;
		}

		public override string DisplayName { get; }

        public override Type ComponentType => typeof(ArrayRowView);

        public override bool IsReadOnly => false;

        public override Type PropertyType { get; }

        public override object GetValue(object component)
		{
			try
			{
				return ((ArrayRowView)component).GetColumn(_index);
			}
			catch(Exception e)
			{
				Debug.WriteLine(e);
			}

			Debug.Assert(false);

			return null;
		}

        public override void ResetValue(object component)
        {
            
        }

        public override void SetValue(object component, object value)
		{
			try
			{
				((ArrayRowView)component).SetColumnValue(_index,value);
			}
			catch(Exception e)
			{
				Debug.WriteLine(e);
				Debug.Assert(false);
			}

			
		}

		public override bool CanResetValue(object component)
		{
			return false;
		}
		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}
	}// END CLASS DEFINITION ArrayPropertyDescriptor

} // Mommo.Data
