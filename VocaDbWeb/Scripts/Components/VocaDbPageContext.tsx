import React from 'react';

export interface VocaDbPageProps {}

export interface VocaDbPage {
  props: VocaDbPageProps;
}

const VocaDbPageContext = React.createContext<VocaDbPage>(undefined!);

export default VocaDbPageContext;
