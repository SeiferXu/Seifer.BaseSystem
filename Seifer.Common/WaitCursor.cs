using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Seifer.Common
{
    public class WaitCursor : IDisposable
    {
        // Fields
        private Form mFrm;
        private Cursor mOldCursor;
        private bool setCurror;

        // Methods
        public WaitCursor()
        {
            this.setCurror = true;
            this.mOldCursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
        }

        public WaitCursor(object winObj)
        {
            if (winObj != null)
            {
                this.mFrm = winObj as Form;
                this.mFrm.Cursor = Cursors.WaitCursor;
            }
        }

        public WaitCursor(Cursor newCursor, object winObj)
        {
            if (winObj != null)
            {
                this.mFrm = winObj as Form;
                if (newCursor == null)
                {
                    this.mFrm.Cursor = Cursors.WaitCursor;
                }
                else
                {
                    this.mFrm.Cursor = newCursor;
                }
            }
        }

        public void Dispose()
        {
            if (this.mFrm != null)
            {
                if (this.mOldCursor != null)
                {
                    this.mFrm.Cursor = this.mOldCursor;
                    if (this.setCurror)
                    {
                        Cursor.Current = this.mOldCursor;
                    }
                }
                else
                {
                    this.mFrm.Cursor = Cursors.Arrow;
                }
            }
        }

        // Properties
        public Cursor OldCursor
        {
            set
            {
                this.mOldCursor = value;
            }
        }
    }
}
