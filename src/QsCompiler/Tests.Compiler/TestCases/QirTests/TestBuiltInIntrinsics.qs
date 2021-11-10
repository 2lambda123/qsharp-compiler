// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Quantum.Testing.QIR {
    open Microsoft.Quantum.Intrinsic;
    open Microsoft.Quantum.Diagnostics;

    newtype Options = (
        SimpleMessage: (String -> Unit),
        DumpToFile: (String -> Unit),
        DumpToConsole: (Unit -> Unit)
    );

    function Ignore<'T> (arg : 'T) : Unit {}

    function DefaultOptions() : Options {
        return Options(
            Ignore,
            Ignore,
            Ignore
        );
    }

    @EntryPoint()
    operation TestBuiltInIntrinsics() : Unit {
        let options = DefaultOptions()
            w/ SimpleMessage <- Message
            w/ DumpToFile <- DumpMachine
            w/ DumpToConsole <- DumpMachine;

        let num = GetHardwareCycleCounter() - 5;
        options::SimpleMessage("Hello");
        options::DumpToFile("pathToFile");
        options::DumpToConsole();
    }
}

namespace Microsoft.Quantum.Intrinsic {

    function Message (arg : String) : Unit {
        body intrinsic;
    }
}

namespace Microsoft.Quantum.Diagnostics {

    function DumpMachine<'T> (arg : 'T) : Unit {
        body intrinsic;
    }
}

namespace Microsoft.Quantum.Core {
    operation GetHardwareCycleCounter() : Int {
        body intrinsic;
    }
}
