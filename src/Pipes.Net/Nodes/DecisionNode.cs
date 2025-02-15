﻿namespace Pipes.Net.Nodes
{

    public class DecisionNode : PipelineNode
    {
        public static DecisionNode Create<T>(Func<T, bool?> predicate, INode? success, INode? failure)
        {
            var tmp = (object? input) =>
            {
                var args = TypeConverter.Convert<T>(input);

                return predicate(args);
            };

            return new DecisionNode(tmp, success, failure);
        }

        public INode? SuccessNode { get; private set; }
        public INode? FailureNode { get; private set; }

        public Func<object?, bool?> Determination { get; set; }

        public DecisionNode(Func<object?, bool?> determination)
        {
            Determination = determination;
        }

        public DecisionNode(Func<object?, bool?> determination, INode? success, INode? failure)
        {
            Determination = determination;
            SetSuccess(success);
            SetFailure(failure);
        }

        public override async Task<object?> Run(object? input)
        {
            INode? node = null;
            var result = Determination(input);
            if (result is null)
            {
                return input;
            }
            else
            {
                if (result.Value)
                {
                    node = SuccessNode;
                }
                else
                {
                    node = FailureNode;
                }

                if (node is not null)
                {
                    return await node.Run(input);
                }
                else
                {
                    return input;
                }
            }




        }

        public void SetSuccess(INode? node)
        {
            if (node is not null)
            {
                node.Parent = this;
                node.SplitExecutionTreeRoot = true;
            }
            SuccessNode = node;
        }

        public void SetFailure(INode? node)
        {
            if (node is not null)
            {
                node.Parent = this;
                node.SplitExecutionTreeRoot = true;
            }
            FailureNode = node;
        }
    }




}