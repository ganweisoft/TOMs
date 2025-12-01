// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Ganweisoft.IoTCenter.Module.EquipLink.Data;
using Ganweisoft.IoTCenter.Module.EquipLink.Models.Condition;
using System;
using System.Text;

namespace Ganweisoft.IoTCenter.Module.EquipLink
{
    public static class ExpressionGenerator
    {
        public static string GetExpression(this AddConditionModel expression)
        {
            if(expression == null)
                return string.Empty;

            var expSb = new StringBuilder();

            foreach (var conditionItem in expression.IConditionItems)
            {
                foreach (var ycYxItem in conditionItem.IYcYxItems)
                {
                    expSb.Append("(");
                    switch (ycYxItem.IycyxType.ToLower())
                    {
                        case "c":
                            expSb.Append($"$C1({conditionItem.IequipNo},{ycYxItem.IycyxNo})");
                            break;
                        case "x":
                            expSb.Append($"$X1({conditionItem.IequipNo},{ycYxItem.IycyxNo})");
                            break;
                        case "e":
                            expSb.Append($"$E({conditionItem.IequipNo})");
                            break;
                    }

                    switch (ycYxItem.Condition)
                    {
                        case ConditionExpression.Equal:
                            expSb.Append($"=={ycYxItem.IycyxValue}");
                            break;
                        case ConditionExpression.NotEqual:
                            expSb.Append($"!={ycYxItem.IycyxValue}");
                            break;
                        case ConditionExpression.GreaterThan:
                            expSb.Append($">{ycYxItem.IycyxValue}");
                            break;
                        case ConditionExpression.GreaterThanOrEqual:
                            expSb.Append($">={ycYxItem.IycyxValue}");
                            break;
                        case ConditionExpression.LessThan:
                            expSb.Append($"<{ycYxItem.IycyxValue}");
                            break;
                        case ConditionExpression.LessThanOrEqual:
                            expSb.Append($"<={ycYxItem.IycyxValue}");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(ycYxItem.Condition));
                    }

                    expSb.Append(")&&");
                }
            }

            return expSb.ToString(0, expSb.Length - 2);
        }

        public static string GetExpression(this EditConditionModel expression)
        {
            var addModel = new AddConditionModel
            {
                IConditionItems = expression.IConditionItems
            };

            return addModel.GetExpression();
        }
    }
}