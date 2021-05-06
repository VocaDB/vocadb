import React from 'react';

export interface VocaDbPageProps {}

export interface VocaDbPage {
  props: VocaDbPageProps;
}

const VocaDbPageContext = React.createContext<VocaDbPage>(undefined!);

export const useVocaDbPage = (): VocaDbPage =>
  React.useContext(VocaDbPageContext);

interface VocaDbPageProviderProps {
  children?: React.ReactNode;
  value: VocaDbPage;
}

const VocaDbPageProvider = ({
  children,
  value,
}: VocaDbPageProviderProps): React.ReactElement => {
  return (
    <VocaDbPageContext.Provider value={value}>
      {children}
    </VocaDbPageContext.Provider>
  );
};

export default VocaDbPageProvider;
