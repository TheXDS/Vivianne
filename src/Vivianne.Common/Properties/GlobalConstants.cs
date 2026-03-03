using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheXDS.Vivianne.Properties;

public static class GlobalConstants
{
    public static readonly string[] KnownNfs2ProcesNames = [
        "nfs2se-gl1",
        "nfs2se",
        "nfs2sea",
        "nfs2-gl1",
        "nfs2",
        "nfs2a",
    ];

    public static readonly string[] KnownNfsProcessNames = [
        .. KnownNfs2ProcesNames,
        "nfs3",
        "nfs4"];
}
