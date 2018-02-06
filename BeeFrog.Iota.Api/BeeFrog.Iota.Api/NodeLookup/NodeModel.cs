using System;
using System.Collections.Generic;
using System.Text;

namespace BeeFrog.Iota.Api.NodeLookup
{
    public class Node
    {
        public string node { get; set; }
        public string port { get; set; }
        public int? tips { get; set; }
        public string version { get; set; }
        public int? index { get; set; }
        public int? solid { get; set; }
        public int? neighbors { get; set; }
        public bool pow { get; set; }
        public int? load { get; set; }
        public int health { get; set; }

        /// <summary>
        /// Returns back the URL with the port to use.
        /// </summary>
        public string Url { get => $"{this.node}:{this.port}"; }
    }
}
