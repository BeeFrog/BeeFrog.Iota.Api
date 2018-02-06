using BeeFrog.Iota.Api.Crypto;
using BeeFrog.Iota.Api.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeeFrog.Iota.Api.Models
{
    public class AddressModelBase : ModelBase
    {
        private string address;
        private string addressWithCheckSum;

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public string Address
        {
            get
            {
                return this.address;
            }
            set
            {
                if (value != this.address)
                {
                    this.address = value;
                    this.addressWithCheckSum = Checksum.AddChecksum(address);
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(AddressWithCheckSum));
                }
            }
        }

        public string AddressWithCheckSum
        {
            get
            {
                return this.addressWithCheckSum;
            }
        }
    }
}
