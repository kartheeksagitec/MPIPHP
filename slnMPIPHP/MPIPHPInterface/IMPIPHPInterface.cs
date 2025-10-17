#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using Sagitec.Interface;

#endregion

namespace MPIPHP.Interface
{
	public interface IMPIPHPDBCache: IDBCache
	{
        string GetMPIPHPMessageDescription(int aintMessageID);
	}

	public interface IMPIPHPMetaDataCache: IMetaDataCache
	{
	}

	public interface IMPIPHPBusinessTier: IBusinessTier
	{
	}
}