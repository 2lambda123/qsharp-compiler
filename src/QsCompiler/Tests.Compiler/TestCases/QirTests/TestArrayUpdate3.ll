define internal %Array* @Microsoft__Quantum__Testing__QIR__TestArrayUpdate3__body(%Array* %y, %String* %b) {
entry:
  %arr = alloca %Array*, align 8
  %x = alloca %Array*, align 8
  call void @__quantum__rt__array_update_alias_count(%Array* %y, i32 1)
  store %Array* %y, %Array** %x, align 8
  call void @__quantum__rt__array_update_alias_count(%Array* %y, i32 1)
  %0 = call i64 @__quantum__rt__array_get_size_1d(%Array* %y)
  %1 = sub i64 %0, 1
  br label %header__1

header__1:                                        ; preds = %exiting__1, %entry
  %2 = phi i64 [ 0, %entry ], [ %7, %exiting__1 ]
  %3 = icmp sle i64 %2, %1
  br i1 %3, label %body__1, label %exit__1

body__1:                                          ; preds = %header__1
  %4 = call i8* @__quantum__rt__array_get_element_ptr_1d(%Array* %y, i64 %2)
  %5 = bitcast i8* %4 to %String**
  %6 = load %String*, %String** %5, align 8
  call void @__quantum__rt__string_update_reference_count(%String* %6, i32 1)
  br label %exiting__1

exiting__1:                                       ; preds = %body__1
  %7 = add i64 %2, 1
  br label %header__1

exit__1:                                          ; preds = %header__1
  call void @__quantum__rt__array_update_reference_count(%Array* %y, i32 1)
  call void @__quantum__rt__array_update_alias_count(%Array* %y, i32 -1)
  %8 = call %Array* @__quantum__rt__array_copy(%Array* %y, i1 false)
  %9 = call i8* @__quantum__rt__array_get_element_ptr_1d(%Array* %8, i64 0)
  %10 = bitcast i8* %9 to %String**
  call void @__quantum__rt__string_update_reference_count(%String* %b, i32 1)
  %11 = load %String*, %String** %10, align 8
  call void @__quantum__rt__string_update_reference_count(%String* %11, i32 -1)
  store %String* %b, %String** %10, align 8
  call void @__quantum__rt__array_update_alias_count(%Array* %8, i32 1)
  store %Array* %8, %Array** %x, align 8
  call void @__quantum__rt__array_update_alias_count(%Array* %8, i32 -1)
  %12 = call %Array* @__quantum__rt__array_copy(%Array* %8, i1 false)
  %13 = call %String* @__quantum__rt__string_create(i8* getelementptr inbounds ([6 x i8], [6 x i8]* @2, i32 0, i32 0))
  %14 = call i8* @__quantum__rt__array_get_element_ptr_1d(%Array* %12, i64 1)
  %15 = bitcast i8* %14 to %String**
  %16 = load %String*, %String** %15, align 8
  call void @__quantum__rt__string_update_reference_count(%String* %16, i32 -1)
  store %String* %13, %String** %15, align 8
  call void @__quantum__rt__array_update_alias_count(%Array* %12, i32 1)
  store %Array* %12, %Array** %x, align 8
  %17 = call %Tuple* @__quantum__rt__tuple_create(i64 ptrtoint ({ i64, i64 }* getelementptr ({ i64, i64 }, { i64, i64 }* null, i32 1) to i64))
  %18 = bitcast %Tuple* %17 to { i64, i64 }*
  %19 = getelementptr inbounds { i64, i64 }, { i64, i64 }* %18, i32 0, i32 0
  %20 = getelementptr inbounds { i64, i64 }, { i64, i64 }* %18, i32 0, i32 1
  store i64 0, i64* %19, align 4
  store i64 0, i64* %20, align 4
  %21 = call %Array* @__quantum__rt__array_create_1d(i32 8, i64 10)
  %22 = call i8* @__quantum__rt__array_get_element_ptr_1d(%Array* %21, i64 0)
  %23 = bitcast i8* %22 to { i64, i64 }**
  %24 = call i8* @__quantum__rt__array_get_element_ptr_1d(%Array* %21, i64 1)
  %25 = bitcast i8* %24 to { i64, i64 }**
  %26 = call i8* @__quantum__rt__array_get_element_ptr_1d(%Array* %21, i64 2)
  %27 = bitcast i8* %26 to { i64, i64 }**
  %28 = call i8* @__quantum__rt__array_get_element_ptr_1d(%Array* %21, i64 3)
  %29 = bitcast i8* %28 to { i64, i64 }**
  %30 = call i8* @__quantum__rt__array_get_element_ptr_1d(%Array* %21, i64 4)
  %31 = bitcast i8* %30 to { i64, i64 }**
  %32 = call i8* @__quantum__rt__array_get_element_ptr_1d(%Array* %21, i64 5)
  %33 = bitcast i8* %32 to { i64, i64 }**
  %34 = call i8* @__quantum__rt__array_get_element_ptr_1d(%Array* %21, i64 6)
  %35 = bitcast i8* %34 to { i64, i64 }**
  %36 = call i8* @__quantum__rt__array_get_element_ptr_1d(%Array* %21, i64 7)
  %37 = bitcast i8* %36 to { i64, i64 }**
  %38 = call i8* @__quantum__rt__array_get_element_ptr_1d(%Array* %21, i64 8)
  %39 = bitcast i8* %38 to { i64, i64 }**
  %40 = call i8* @__quantum__rt__array_get_element_ptr_1d(%Array* %21, i64 9)
  %41 = bitcast i8* %40 to { i64, i64 }**
  store { i64, i64 }* %18, { i64, i64 }** %23, align 8
  store { i64, i64 }* %18, { i64, i64 }** %25, align 8
  store { i64, i64 }* %18, { i64, i64 }** %27, align 8
  store { i64, i64 }* %18, { i64, i64 }** %29, align 8
  store { i64, i64 }* %18, { i64, i64 }** %31, align 8
  store { i64, i64 }* %18, { i64, i64 }** %33, align 8
  store { i64, i64 }* %18, { i64, i64 }** %35, align 8
  store { i64, i64 }* %18, { i64, i64 }** %37, align 8
  store { i64, i64 }* %18, { i64, i64 }** %39, align 8
  store { i64, i64 }* %18, { i64, i64 }** %41, align 8
  call void @__quantum__rt__tuple_update_reference_count(%Tuple* %17, i32 1)
  call void @__quantum__rt__tuple_update_reference_count(%Tuple* %17, i32 1)
  call void @__quantum__rt__tuple_update_reference_count(%Tuple* %17, i32 1)
  call void @__quantum__rt__tuple_update_reference_count(%Tuple* %17, i32 1)
  call void @__quantum__rt__tuple_update_reference_count(%Tuple* %17, i32 1)
  call void @__quantum__rt__tuple_update_reference_count(%Tuple* %17, i32 1)
  call void @__quantum__rt__tuple_update_reference_count(%Tuple* %17, i32 1)
  call void @__quantum__rt__tuple_update_reference_count(%Tuple* %17, i32 1)
  call void @__quantum__rt__tuple_update_reference_count(%Tuple* %17, i32 1)
  call void @__quantum__rt__tuple_update_reference_count(%Tuple* %17, i32 1)
  store %Array* %21, %Array** %arr, align 8
  br label %header__2

header__2:                                        ; preds = %exiting__2, %exit__1
  %42 = phi i64 [ 0, %exit__1 ], [ %48, %exiting__2 ]
  %43 = icmp sle i64 %42, 9
  br i1 %43, label %body__2, label %exit__2

body__2:                                          ; preds = %header__2
  %44 = call i8* @__quantum__rt__array_get_element_ptr_1d(%Array* %21, i64 %42)
  %45 = bitcast i8* %44 to { i64, i64 }**
  %46 = load { i64, i64 }*, { i64, i64 }** %45, align 8
  %47 = bitcast { i64, i64 }* %46 to %Tuple*
  call void @__quantum__rt__tuple_update_alias_count(%Tuple* %47, i32 1)
  br label %exiting__2

exiting__2:                                       ; preds = %body__2
  %48 = add i64 %42, 1
  br label %header__2

exit__2:                                          ; preds = %header__2
  call void @__quantum__rt__array_update_alias_count(%Array* %21, i32 1)
  br label %header__3

header__3:                                        ; preds = %exiting__3, %exit__2
  %i = phi i64 [ 0, %exit__2 ], [ %61, %exiting__3 ]
  %49 = icmp sle i64 %i, 9
  br i1 %49, label %body__3, label %exit__3

body__3:                                          ; preds = %header__3
  %50 = load %Array*, %Array** %arr, align 8
  call void @__quantum__rt__array_update_alias_count(%Array* %50, i32 -1)
  %51 = call %Array* @__quantum__rt__array_copy(%Array* %50, i1 false)
  %52 = add i64 %i, 1
  %53 = call %Tuple* @__quantum__rt__tuple_create(i64 ptrtoint ({ i64, i64 }* getelementptr ({ i64, i64 }, { i64, i64 }* null, i32 1) to i64))
  %54 = bitcast %Tuple* %53 to { i64, i64 }*
  %55 = getelementptr inbounds { i64, i64 }, { i64, i64 }* %54, i32 0, i32 0
  %56 = getelementptr inbounds { i64, i64 }, { i64, i64 }* %54, i32 0, i32 1
  store i64 %i, i64* %55, align 4
  store i64 %52, i64* %56, align 4
  %57 = call i8* @__quantum__rt__array_get_element_ptr_1d(%Array* %51, i64 %i)
  %58 = bitcast i8* %57 to { i64, i64 }**
  call void @__quantum__rt__tuple_update_alias_count(%Tuple* %53, i32 1)
  %59 = load { i64, i64 }*, { i64, i64 }** %58, align 8
  %60 = bitcast { i64, i64 }* %59 to %Tuple*
  call void @__quantum__rt__tuple_update_alias_count(%Tuple* %60, i32 -1)
  call void @__quantum__rt__tuple_update_reference_count(%Tuple* %60, i32 -1)
  store { i64, i64 }* %54, { i64, i64 }** %58, align 8
  call void @__quantum__rt__array_update_alias_count(%Array* %51, i32 1)
  store %Array* %51, %Array** %arr, align 8
  call void @__quantum__rt__array_update_reference_count(%Array* %50, i32 -1)
  br label %exiting__3

exiting__3:                                       ; preds = %body__3
  %61 = add i64 %i, 1
  br label %header__3

exit__3:                                          ; preds = %header__3
  %62 = load %Array*, %Array** %arr, align 8
  call void @__quantum__rt__array_update_alias_count(%Array* %y, i32 -1)
  call void @__quantum__rt__array_update_alias_count(%Array* %12, i32 -1)
  %63 = call i64 @__quantum__rt__array_get_size_1d(%Array* %62)
  %64 = sub i64 %63, 1
  br label %header__4

header__4:                                        ; preds = %exiting__4, %exit__3
  %65 = phi i64 [ 0, %exit__3 ], [ %71, %exiting__4 ]
  %66 = icmp sle i64 %65, %64
  br i1 %66, label %body__4, label %exit__4

body__4:                                          ; preds = %header__4
  %67 = call i8* @__quantum__rt__array_get_element_ptr_1d(%Array* %62, i64 %65)
  %68 = bitcast i8* %67 to { i64, i64 }**
  %69 = load { i64, i64 }*, { i64, i64 }** %68, align 8
  %70 = bitcast { i64, i64 }* %69 to %Tuple*
  call void @__quantum__rt__tuple_update_alias_count(%Tuple* %70, i32 -1)
  br label %exiting__4

exiting__4:                                       ; preds = %body__4
  %71 = add i64 %65, 1
  br label %header__4

exit__4:                                          ; preds = %header__4
  call void @__quantum__rt__array_update_alias_count(%Array* %62, i32 -1)
  call void @__quantum__rt__array_update_reference_count(%Array* %y, i32 -1)
  call void @__quantum__rt__array_update_reference_count(%Array* %8, i32 -1)
  call void @__quantum__rt__tuple_update_reference_count(%Tuple* %17, i32 -1)
  %72 = sub i64 %63, 1
  br label %header__5

header__5:                                        ; preds = %exiting__5, %exit__4
  %73 = phi i64 [ 0, %exit__4 ], [ %79, %exiting__5 ]
  %74 = icmp sle i64 %73, %72
  br i1 %74, label %body__5, label %exit__5

body__5:                                          ; preds = %header__5
  %75 = call i8* @__quantum__rt__array_get_element_ptr_1d(%Array* %62, i64 %73)
  %76 = bitcast i8* %75 to { i64, i64 }**
  %77 = load { i64, i64 }*, { i64, i64 }** %76, align 8
  %78 = bitcast { i64, i64 }* %77 to %Tuple*
  call void @__quantum__rt__tuple_update_reference_count(%Tuple* %78, i32 -1)
  br label %exiting__5

exiting__5:                                       ; preds = %body__5
  %79 = add i64 %73, 1
  br label %header__5

exit__5:                                          ; preds = %header__5
  call void @__quantum__rt__array_update_reference_count(%Array* %62, i32 -1)
  ret %Array* %12
}
