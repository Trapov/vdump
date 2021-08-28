import React, {useState} from 'react';
import {
  DetailsList,
  FontWeights,
  IStackStyles,
  IStackTokens,
  ITextStyles,
  Stack,
  Text
} from '@fluentui/react';
import {useHistory} from "react-router-dom";

import {DefaultButton} from '@fluentui/react/lib/Button';

import './App.css';
import api from './api';
import {AppError} from "./components/AppError";
import {ProgressIndicatorIndeterminateExample} from "./components/Progress";
import {GraphView} from "./components/GraphView";

const boldStyle: Partial<ITextStyles> = {root: {fontWeight: FontWeights.semibold}};
const stackTokens: IStackTokens = {childrenGap: 15};
const stackStyles: Partial<IStackStyles> = {
  root: {
    width: '450px',
    margin: '0 auto',
    textAlign: 'center',
    color: '#605e5c',
  },
};


const upload = async (file: File) => {
  return await api.dumpsApi.dump(file);
}

export const App: React.FunctionComponent = () => {
  const history = useHistory();
  const fileUploadRef = React.useRef<HTMLInputElement>(null);
  const [file, setFile] = useState<File | undefined>()
  const [graphView, setGraphView] = useState<any>(undefined)
  const [error, setError] = useState<string | undefined>()

  return graphView == undefined ?
    <Stack horizontalAlign="center" verticalAlign="center" verticalFill styles={stackStyles} tokens={stackTokens}>
      {file ? <ProgressIndicatorIndeterminateExample/> :
        <>
          <Text variant="xxLarge" styles={boldStyle}>
            Drop your gcdump here
          </Text>
          <DefaultButton text={(file as unknown as File)?.name ?? "..."} onClick={() => fileUploadRef.current?.click()}>
            <input onChange={async x => {
              try {
                setFile(x.target.files!.item(0)!);
                const result = await upload(x.target.files!.item(0)!)
                setGraphView(result);
                history?.push(`/${result.id}`)
              } catch (e) {
                setError(e.message);
                setFile(undefined)
              }
            }} style={{display: 'none'}} type="file" ref={fileUploadRef}/>
          </DefaultButton>
        </>
      }
      {error ? <AppError error={error} onDismiss={() => setError(undefined)}/> : <></>}

    </Stack> : <GraphView graph={graphView}/>;
}
