using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace BeeFrog.Iota.Api.Models
{
    /// <summary>
    /// This class represents an AddressInfo
    /// </summary>
    public class AddressItem : AddressModelBase
    {
        private sbyte[] privateKey;
        private long balance;
        private int keyIndex;

        public AddressItem()
        {
            this.Transactions = new ObservableCollection<TransactionHash>();
            this.Transactions.CollectionChanged += Transactions_CollectionChanged;
        }

        private void Transactions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(TransactionCount));
        }

        public int TransactionCount => this.Transactions?.Count ?? 0;

        /// <summary>
        /// Gets or sets the private key
        /// </summary>
        public virtual sbyte[] PrivateKey { get; internal set; }

        /// <summary>
        /// Returns the private key as Trints
        /// This may not work!
        /// </summary>
        public int[] PrivateKeyTrints
        {
            get
            {
                var trytes = Utils.Converter.GetTrytes(this.PrivateKey);
                var response = Utils.Converter.ToTrits(trytes);
                return response;
            }
        }

        /// <summary>
        /// Gets or sets the balance.
        /// </summary>
        /// <value>
        /// The balance.
        /// </value>
        public long Balance { get; internal set; }

        /// <summary>
        /// Gets or sets the index of the key.
        /// </summary>
        /// <value>
        /// The index of the key.
        /// </value>
        public int Index { get; internal set; }

        /// <summary>
        /// Gets the transactions
        /// </summary>
        public ObservableCollection<TransactionHash> Transactions { get; }
    }
}
