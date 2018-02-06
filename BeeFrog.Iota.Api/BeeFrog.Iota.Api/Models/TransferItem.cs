using System;
using System.Collections.Generic;
using System.Text;

namespace BeeFrog.Iota.Api.Models
{
    /// <summary>
    /// This class represents a Transfer
    /// </summary>
    public class TransferItem : AddressModelBase
    {
        private long value;
        private string message;
        private string tag;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferItem"/> class.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="value">The value.</param>
        /// <param name="message">The message.</param>
        /// <param name="tag">The tag.</param>
        public TransferItem()
        {
        }        

        /// <summary>
        /// Gets or sets the value in iota. (e.g 1 = 1 iota, 1000000 = 1 MIota)
        /// </summary>
        public long Value
        {
            get
            {
                return this.value;
            }
            set
            {
                if (value != this.value)
                {
                    this.value = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the message in Trytes (A-Z and 9).
        /// See <see cref="Utils.Converter.AsciiToTrytes"/> to convert from ASCII message.
        /// This can be up to X in length.
        /// </summary>        
        public string Message
        {
            get
            {
                
                return this.message;
            }
            set
            {
                if (value != this.message)
                {
                    this.message = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>
        /// The tag.
        /// </value>
        public string Tag
        {
            get
            {
                return this.tag;
            }
            set
            {
                if (value != this.tag)
                {
                    this.tag = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(Address)}: {Address}, {nameof(Message)}: {Message}, {nameof(Tag)}: {Tag}, {nameof(Value)}: {Value}";
        }
    }
}
