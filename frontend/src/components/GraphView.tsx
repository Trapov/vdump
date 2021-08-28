import React, {useState} from "react";
import {DetailsList} from "@fluentui/react";

export const GraphView: React.FunctionComponent<{ graph: any }> = ({graph}) => {
  const [items, setItems] = useState(graph.reportItems);
  const [columns, setColumns] = useState<Array<{ name: string, isSortedDescending: boolean }>>([])

  function _copyAndSort<T>(i: T[], columnKey: string, isSortedDescending?: boolean): T[] {
    const key = columnKey as keyof T;
    return i.slice(0).sort((a: T, b: T) => {
      return ((isSortedDescending ? a[key] < b[key] : a[key] > b[key]) ? 1 : -1);
    });
  }

  return (
    <DetailsList onColumnHeaderClick={(ev, column) => {

      let c = columns.find(x => x.name == column!.name);
      let newColumns = columns;
      if (!c) {
        c = {
          name: column!.name,
          isSortedDescending: true
        }
        newColumns = [c, ...columns]
      }

      setItems(_copyAndSort(items, column!.fieldName!, c.isSortedDescending))

      c.isSortedDescending = !c.isSortedDescending;
      setColumns(newColumns);

    }} items={items}>
    </DetailsList>
  )
};
