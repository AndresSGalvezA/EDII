namespace L3.Models
{
    public class HuffmanNode
    {
        public byte Fact { get; set; }
        public decimal Probability { get; set; }
        public HuffmanNode Parent { get; set; }
        public HuffmanNode LeftNode { get; set; }
        public HuffmanNode RightNode { get; set; }

        public HuffmanNode(byte _fact, decimal _probability)
        {
            Fact = _fact;
            Probability = _probability;
            LeftNode = null;
            RightNode = null;
            Parent = null;
        }

        public HuffmanNode(decimal _probability)
        {
            Probability = _probability;
            LeftNode = null;
            RightNode = null;
            Parent = null;
        }

        public bool IsLeaf()
        {
            return (RightNode == null && LeftNode == null) ? true : false;
        }
    }
}
