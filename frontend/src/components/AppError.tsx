import React from "react";
import {FontWeights, ITextStyles, MessageBar, MessageBarType, Stack, Text} from "@fluentui/react";

const boldStyle: Partial<ITextStyles> = {root: {fontWeight: FontWeights.semibold}};

export const AppError: React.FunctionComponent<{ error: string, onDismiss: CallableFunction }> = ({error, onDismiss}) => {
  return (
    <MessageBar
      messageBarType={MessageBarType.error}
      onDismiss={() => onDismiss()}
      dismissButtonAriaLabel="Close"
    >
      <Stack>
        <Text variant="mediumPlus" styles={boldStyle}> {error} </Text>
      </Stack>
    </MessageBar>
  )
};
