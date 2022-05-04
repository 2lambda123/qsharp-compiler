﻿namespace Microsoft.Quantum.Testing.ExecutionTests {

    open Microsoft.Quantum.Intrinsic;

    function SumArray(arr : Int[]) : Int {
        mutable sum = 0;
        for item in arr{
            set sum += item;
        }
        return sum;
    }

    @EntryPoint()
    operation TestNativeTypeHandling() : Result {
        let arr1 = [1,2,3];
        Message($"{arr1}");

        let sum = SumArray(arr1);
        let arr2 = [sum, size = 3];
        Message($"{arr2}");

        for i in 0 .. Length(arr1)-1 {
            Message($"item {i} is {arr1[i]}");
        }

        //let tupleArr = [(PauliX, 0), (PauliZ, 1), (PauliY, 2)]; // FIXME INCORRECT TYPES...
        //let (pauli, _) = tupleArr[1];
        //Message($"{pauli}");

        return Zero;
    }


    //@EntryPoint()
    //operation TestArraySlicing() : Range {
      //
      //  mutable arr = [1,2,3,4];
      //  Message($"{arr}, {arr[...-1...]}");
      //  mutable value = arr;
      //  let check1 = value;
      //  mutable check2 = arr;
      //
      //  if arr[0] == 1 {
      //      set arr w/= 3..-1..0 <- arr;
      //      Message($"{arr}, {value}");
      //      set value w/= 0..2..2 <- arr[Length(arr)/2...];
      //      Message($"{arr}, {value}");
      //      set arr w/= 0..-1 <- [];
      //      Message($"{arr}, {value}");
      //  }
      //
      //  mutable arrarr = [[], [0], [1]];
      //  set arrarr w/= 2..-1..0 <- arrarr;
      //  let iter = [[1],[2],[3]];
      //  Message($"{arrarr}, {iter w/ 2..-1..0 <- iter}");
      //
      //  set arrarr = value[0] == 1
      //      ? [[10], size = 5] w/ 4..-2..-1 <- [[6], size = 3]
      //      | (iter w/ 0..2..3 <- arrarr[1...]);
      //  Message($"{arrarr}, {iter}");
      //
      //  set arrarr = value[0] != 1
      //      ? [[10], size = 5] w/ 4..-2..-1 <- [[5], size = 3]
      //      | (iter w/ 0..2..3 <- arrarr[1...]);
      //  Message($"{arrarr}, {iter}");
      //
      //  set arrarr = [[0], [0,0], [1,1,1]];
      //  Message($"{arrarr}");
      //  set arrarr w/= 1..-1..0 <- arrarr[0..1];
      //  Message($"{arrarr}");
      //  set arrarr w/= 2..-1..0 <- arrarr;
      //  Message($"{arrarr}");
      //  set arrarr w/= 0..2..3 <- arrarr[0..2..3][...-1...];
      //  Message($"{arrarr}");
      //
      //  Message($"{check1}, {check2}");
      //  return 1..3..5;
    //}
}

namespace Microsoft.Quantum.Intrinsic {

    function Message (arg : String) : Unit {
        body intrinsic;
    }
}