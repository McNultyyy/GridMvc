﻿using System;

namespace GridMvc.Filtering.Types
{
    /// <summary>
    /// Object contains some logic for filtering Int32 columns
    /// </summary>
    internal sealed class IntegerFilterType : FilterTypeBase
    {
        public override Type TargetType
        {
            get { return typeof (Int32); }
        }

        public override GridFilterType GetValidType(GridFilterType type)
        {
            switch (type)
            {
                case GridFilterType.Equals:
                case GridFilterType.GreaterThan:
                case GridFilterType.LessThan:
                    return type;
                default:
                    return GridFilterType.Equals;
            }
        }

        public override object GetTypedValue(string value)
        {
            int i;
            if (!int.TryParse(value, out i))
                return null;
            return i;
        }
    }
}